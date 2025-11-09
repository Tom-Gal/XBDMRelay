using System.Text;
using System.Text.RegularExpressions;

namespace XBDMRelay.Parsers
{
    /*
     *  This is what makes the responses look pretty. There is much to be improved here so by all means, feel free to contribute. 
     *  I will try to keep this updated as best as I can when I find time.
     */
    public class XBDMParser
    {
        public string ParseXboxCommand(string line, string direction)
        {
            if (string.IsNullOrWhiteSpace(line)) return string.Empty;
            string lowerLine = line.ToLowerInvariant();
            string result = string.Empty;

            try
            {
                if (lowerLine.StartsWith("consolefeatures"))
                    result = ParseConsoleFeatures(line, direction);
                else if (lowerLine.StartsWith("setmem"))
                    result = ParseSetMem(line, direction);
                else if (lowerLine.StartsWith("getmem"))
                    result = ParseGetMem(line, direction);
                else if (lowerLine.StartsWith("magicboot") || lowerLine.StartsWith("callvoid"))
                    result = ParseCallVoid(line, direction);
                else if (lowerLine.StartsWith("call"))
                    result = ParseCall(line, direction);
                else if (lowerLine.StartsWith("syscall") || lowerLine.StartsWith("krnl"))
                    result = ParseKernelCall(line, direction);
                else if (lowerLine.StartsWith("getfile") || lowerLine.StartsWith("sendfile"))
                    result = ParseFileOperation(line, direction);
                else if (lowerLine.StartsWith("getmodules") || lowerLine.StartsWith("modload") || lowerLine.StartsWith("modunload"))
                    result = ParseModuleOperation(line, direction);
                else if (lowerLine.StartsWith("threads") || lowerLine.StartsWith("suspend") || lowerLine.StartsWith("resume"))
                    result = ParseThreadOperation(line, direction);
                else if (lowerLine.StartsWith("break") || lowerLine.StartsWith("bp"))
                    result = ParseBreakpoint(line, direction);
                else if (lowerLine.StartsWith("dbgname") || lowerLine.StartsWith("dbgoptions") || lowerLine.StartsWith("title") || lowerLine.StartsWith("xbeinfo"))
                    result = ParseDebugCommand(line, direction);
                else if (Regex.IsMatch(line, @"^(200|201|202|203|204|400|401|402|403)"))
                    result = ParseResponse(line, direction);
                else
                    result = $"[{direction}] {line}";
            }
            catch (Exception ex)
            {
                result = $"[{direction}] Error parsing command: {ex.Message}\n[{direction}] Raw: {line}";
            }

            return result;
        }

        private string ParseSetMem(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 3)
            {
                string address = parts[1];
                string length = parts[2];
                string encodedData = parts.Length > 3 ? string.Join(" ", parts.Skip(3)) : null;

                output.AppendLine($"[{direction}] SETMEM Command");
                output.AppendLine($"[{direction}] Address: {address}");
                output.AppendLine($"[{direction}] Length: {length} bytes");

                if (!string.IsNullOrEmpty(encodedData))
                {
                    byte[] decodedBytes = DecodeData(encodedData);
                    if (decodedBytes != null && decodedBytes.Length > 0)
                    {
                        output.AppendLine($"[{direction}] Data ({decodedBytes.Length} bytes): {FormatBytes(decodedBytes)}");
                        string ascii = GetPrintableAscii(decodedBytes);
                        if (!string.IsNullOrEmpty(ascii))
                            output.AppendLine($"[{direction}] ASCII: {ascii}");
                    }
                }
            }

            return output.ToString();
        }

        private string ParseConsoleFeatures(string line, string direction)
        {
            var output = new StringBuilder();
            output.AppendLine($"[{direction}] JRPC CONSOLEFEATURES (CallArgs)");

            try
            {
                var kvPairs = new Dictionary<string, string>();
                var matches = Regex.Matches(line, @"(\w+)=(""[^""]*""|[^\s]+)");
                foreach (Match match in matches)
                    kvPairs[match.Groups[1].Value] = match.Groups[2].Value.Trim('"');

                bool isSystem = line.Contains(" system");
                bool isVM = line.Contains(" VM");

                if (kvPairs.ContainsKey("ver"))
                    output.AppendLine($"[{direction}] JRPC Version: {kvPairs["ver"]}");

                string returnType = kvPairs.ContainsKey("type") ? kvPairs["type"] : "0";
                string typeName = GetJRPCTypeName(returnType);
                output.AppendLine($"[{direction}] Return Type: {returnType} ({typeName})");

                if (isSystem) output.AppendLine($"[{direction}] System Thread: YES");
                if (isVM) output.AppendLine($"[{direction}] VM Mode: YES");

                if (kvPairs.TryGetValue("module", out var module))
                    output.AppendLine($"[{direction}] Module: {module}");
                if (kvPairs.TryGetValue("ord", out var ordinal))
                    output.AppendLine($"[{direction}] Ordinal: {ordinal}");

                if (kvPairs.TryGetValue("as", out var arraySize))
                    output.AppendLine($"[{direction}] Array Size: {arraySize}");

                if (kvPairs.TryGetValue("buf_addr", out var buffer))
                    output.AppendLine($"[{direction}] Buffer Address: 0x{buffer}");

                string functionAddress = null;
                var arguments = new List<string>();
                if (kvPairs.ContainsKey("params"))
                    ParseJRPCParams(kvPairs["params"], direction, out functionAddress, out arguments);

                output.AppendLine($"[{direction}] Function Address: 0x{functionAddress}");
                if (arguments.Any())
                {
                    output.AppendLine($"[{direction}] Arguments:");
                    foreach (var arg in arguments)
                        output.AppendLine($"  {arg}");
                }
            }
            catch (Exception ex)
            {
                output.AppendLine($"[{direction}] Error parsing consolefeatures: {ex.Message}");
                output.AppendLine($"[{direction}] Raw: {line}");
            }

            return output.ToString();
        }

        private void ParseJRPCParams(string paramsString, string direction, out string functionAddress, out List<string> arguments)
        {
            functionAddress = null;
            arguments = new List<string>();
            var parts = paramsString.Split('\\');

            if (parts.Length < 4) return;

            if (parts[0] == "A" && parts.Length > 1)
                functionAddress = parts[1];

            int argStartIndex = 4;
            for (int i = argStartIndex; i < parts.Length - 1; i += 2)
            {
                if (i + 1 >= parts.Length) break;
                string arg = ParseJRPCArgument(parts[i], parts[i + 1]);
                if (!string.IsNullOrEmpty(arg)) arguments.Add(arg);
            }
        }

        private string ParseJRPCArgument(string typeStr, string value)
        {
            return typeStr switch
            {
                "1" => $"0x{int.Parse(value):X}",
                "2" => $"\"{value}\"",
                "3" => $"{value}f",
                "4" => $"(byte){value}",
                "8" => $"{value}UL",
                _ => value
            };
        }

        private string GetJRPCTypeName(string typeCode) => typeCode switch
        {
            "0" => "Void",
            "1" => "Int",
            "2" => "String",
            "3" => "Float",
            "4" => "Byte",
            "5" => "IntArray",
            "6" => "FloatArray",
            "7" => "ByteArray",
            "8" => "UInt64",
            "9" => "UInt64Array",
            _ => "Unknown"
        };

        private string ParseGetMem(string line, string direction)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return string.Empty;

            return $"[{direction}] GETMEM Command\n[{direction}] Address: {parts[1]}\n[{direction}] Length: {parts[2]} bytes";
        }

        private string ParseCallVoid(string line, string direction)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return string.Empty;

            var output = new StringBuilder();
            output.AppendLine($"[{direction}] CALL/MAGICBOOT Command");
            output.AppendLine($"[{direction}] Function Address: {parts[1]}");

            if (parts.Length > 2)
            {
                var args = parts.Skip(2).ToArray();
                output.AppendLine($"[{direction}] Arguments ({args.Length}):");
                for (int i = 0; i < args.Length; i++)
                    output.AppendLine($"[{direction}] Arg{i}: {args[i]}");
            }

            return output.ToString();
        }

        private string ParseCall(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                string funcAddress = parts[1];
                output.AppendLine($"[{direction}] CALL Command");
                output.AppendLine($"[{direction}] Function Address: {funcAddress}");

                if (parts.Length > 2)
                {
                    var args = parts.Skip(2).ToArray();
                    output.AppendLine($"[{direction}] Arguments ({args.Length}):");
                    for (int i = 0; i < args.Length; i++)
                        output.AppendLine($"[{direction}] Arg{i}: {args[i]}");
                }
            }

            return output.ToString();
        }

        private string ParseKernelCall(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                string kernelFunc = parts[1];
                output.AppendLine($"[{direction}] KERNEL Call");
                output.AppendLine($"[{direction}] Kernel Function: {kernelFunc}");

                if (parts.Length > 2)
                {
                    var args = parts.Skip(2).ToArray();
                    output.AppendLine($"[{direction}] Parameters ({args.Length}):");
                    for (int i = 0; i < args.Length; i++)
                        output.AppendLine($"[{direction}] Param{i}: {args[i]}");
                }
            }

            return output.ToString();
        }

        private string ParseFileOperation(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                string operation = parts[0].ToUpper();
                string filename = string.Join(" ", parts.Skip(1));

                output.AppendLine($"[{direction}] FILE Operation");
                output.AppendLine($"[{direction}] Operation: {operation}");
                output.AppendLine($"[{direction}] File: {filename}");
            }

            return output.ToString();
        }

        private string ParseModuleOperation(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string operation = parts[0].ToUpper();

            output.AppendLine($"[{direction}] MODULE Operation");
            output.AppendLine($"[{direction}] Operation: {operation}");

            if (parts.Length > 1)
                output.AppendLine($"[{direction}] Module: {string.Join(" ", parts.Skip(1))}");

            return output.ToString();
        }

        private string ParseThreadOperation(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string operation = parts[0].ToUpper();

            output.AppendLine($"[{direction}] THREAD Operation");
            output.AppendLine($"[{direction}] Operation: {operation}");

            if (parts.Length > 1)
                output.AppendLine($"[{direction}] Thread ID: {parts[1]}");

            return output.ToString();
        }

        private string ParseBreakpoint(string line, string direction)
        {
            var output = new StringBuilder();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                string address = parts[1];
                output.AppendLine($"[{direction}] BREAKPOINT Operation");
                output.AppendLine($"[{direction}] Address: {address}");

                if (parts.Length > 2)
                    output.AppendLine($"[{direction}] Options: {string.Join(" ", parts.Skip(2))}");
            }

            return output.ToString();
        }

        private string ParseDebugCommand(string line, string direction)
        {
            var output = new StringBuilder();
            output.AppendLine($"[{direction}] DEBUG Command");
            output.AppendLine($"[{direction}] {line}");
            return output.ToString();
        }

        private string ParseResponse(string line, string direction)
        {
            string statusCode = line.Length >= 3 ? line.Substring(0, 3) : "000";
            string message = line.Length > 4 ? line.Substring(4) : string.Empty;
            string statusDesc = GetStatusDescription(statusCode);

            var output = new StringBuilder();
            output.AppendLine($"[{direction}] RESPONSE");
            output.AppendLine($"[{direction}] Status: {statusCode} - {statusDesc}");

            if (!string.IsNullOrEmpty(message))
                output.AppendLine($"[{direction}] Message: {message}");

            return output.ToString();
        }

        private string GetStatusDescription(string code) => code switch
        {
            "200" => "OK (Command successful)",
            "201" => "Connected",
            "202" => "Multiline response follows",
            "203" => "Binary response follows",
            "204" => "Send binary data",
            "400" => "Unexpected error",
            "401" => "Max number of connections exceeded",
            "402" => "File not found",
            "403" => "No such module",
            "404" => "Memory not mapped",
            "405" => "No such thread",
            "406" => "File must be copied",
            "407" => "File already exists",
            "408" => "Directory not empty",
            "409" => "Filename is invalid",
            "410" => "File cannot be created",
            "411" => "Access denied",
            _ => "Unknown status"
        };

        private byte[] DecodeData(string encoded)
        {
            try { return Enumerable.Range(0, encoded.Length / 2).Select(x => Convert.ToByte(encoded.Substring(x * 2, 2), 16)).ToArray(); }
            catch { return null; }
        }

        private string FormatBytes(byte[] data) => string.Join(" ", data.Select(b => b.ToString("X2")));
        private string GetPrintableAscii(byte[] bytes) => new string(bytes.Select(b => b >= 32 && b <= 126 ? (char)b : '.').ToArray());
    }
}

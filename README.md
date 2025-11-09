# XBDMRelay

XBDMRelay is an open-source C# app that acts as a network proxy and command parser for RGH/JTAG Xbox consoles. It allows you to monitor all traffic between your PC and Xbox over TCP/UDP, giving you logging of XBDM commands. This makes it useful for analyzing memory access performed by RTM or other xbox modding tools.

<img width="747" height="492" alt="image" src="https://github.com/user-attachments/assets/a7d51685-2530-4d25-9883-76e590ed971a" />

## How to Use

1. Launch XBDMRelay on your PC, either clone and build in Visual Studio, or download from the Releases page.
2. Enter your Xbox IP and port in the provided text fields. (Default port is usually 730)
3. Start Listening by clicking the button.
   - The tool will detect a free IP on that subnet and will allocate it for listening.
4. Use the Proxy IP:
   - Open Xbox 360 Neighborhood on your PC.
   - Replace the console IP with the proxy IP provided by XBDMRelay.
   - Connect to this IP instead of the original console.
5. Monitor Traffic:
   - All TCP/UDP traffic between your PC and the Xbox will now be routed through XBDMRelay.
   - Logs will show parsed Xbox Debug Monitor (XBDM) commands in real time.
6. Stop Listening:
   - Click the button again to stop the proxy when you are done.

This allows you to observe and debug console communications, memory access, and other XBDM interactions safely without connecting directly to the xbox.

## Contributions

Contributions are very much welcome! Feel free to modify the XBDM parser to suit your needs. Pull requests, bug reports, and feature suggestions are all greatly appreciated :)

## Requirements

- .NET Core
- An Xbox Development Kit, or RGH/Jtag

## License

This project is licensed under the GNU General Public License (GPL). You are free to use, modify, and distribute this software under the terms of the GPL. For more details, see the [LICENSE](LICENSE) file included in this repository.

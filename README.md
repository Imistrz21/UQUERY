# XAMPP Control Panel Alternative

A Windows application written in C# that serves as an alternative to the XAMPP Control Panel. This tool provides improved functions and allows users to start and stop Apache, MySQL, FileZilla, Mercury, and Tomcat (Catalina) services with better logging and process management.

## Features
- Start and stop XAMPP services using batch scripts.
- Automatic extraction of `data.zip` for setup.
- Logs service actions with timestamps.
- Detects if a service quits early and updates the button text accordingly.
- Forces a service to stop if it doesn't close within 10 seconds.
- Improved error handling and UI updates.

## Installation
### Option 1: Using Prebuilt Release
1. Download the latest release from the [GitHub Releases](https://github.com/Imistrz21/UQUERY/releases).
2. Extract the contents and run `UQUERY.exe`.

### Option 2: Manually Copy XAMPP Files (its the easiest way to install all dependencies :D)
1. Install XAMPP on your system.
2. Navigate to your XAMPP installation folder (`C:/xampp`).
3. Copy the entire `xampp` folder.
4. Paste it into the application's directory (inside `bin/Debug` or `bin/Release` for development use).
5. Run `UQUERY.exe`.

## Usage
1. Open the application.
2. Click the start button for a service (e.g., Apache, MySQL) to launch it.
3. Click the button again to stop the service.
4. The button text updates based on the service status.
5. The logs display messages in the format:
   ```
   17:39:45  [main]    Control Panel Ready
   ```
6. If a service quits unexpectedly, the button resets to "Start".

## Requirements
- Windows OS
- .NET Framework 4.7.2 or later

## Contributing
Feel free to submit pull requests or report issues to improve this project!

## License
All xampp files ale under the license included in the xampp files, credits also to them for making xampp
My code is under MIT License

## Other
Copyright 2025 Imistrz21

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.




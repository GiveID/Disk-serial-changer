# 🖥️ Disk Serial Changer

A Windows console application that allows users to change their hard disk serial number using `volumeid.exe` from Sysinternals. This tool automates the process of downloading, extracting, and executing `volumeid.exe` with administrative privileges.

## 🚀 Features
- Automatically detects and lists fixed drives.
- Generates a random serial number in `XXXX-XXXX` format.
- Downloads and extracts `VolumeId.exe` if not found.
- Runs with administrative privileges.
- User-friendly console interface with progress indicators.

## 📜 Requirements
- Windows OS
- Administrator privileges
- Internet connection (for downloading `VolumeId.exe` if not present)

## 📥 Installation
1. Clone or download this repository:
   ```sh
   git clone https://github.com/yourusername/DiskSerialChanger.git
   cd DiskSerialChanger
   ```
2. Open the solution in Visual Studio or compile using:
   ```sh
   csc DiskSerialChanger.cs
   ```

## 🔧 Usage
1. Run the program as Administrator:
   ```sh
   DiskSerialChanger.exe
   ```
2. Select the desired drive when prompted.
3. The program will generate a new serial number and apply the change.
4. Restart your computer for changes to take effect.

## 🔒 Important Notes
- Modifying the disk serial number might have implications for software licensing and system integrity. Use this tool responsibly.
- Ensure you have backups before making any changes.
- This tool utilizes `volumeid.exe`, which is a Microsoft Sysinternals utility.

## 🛠️ How It Works
- **Admin Check**: Ensures the program is running with administrative privileges.
- **VolumeId Validation**: Checks for `volumeid.exe`, downloads it if missing.
- **Drive Selection**: Lists fixed drives and allows the user to select one.
- **Serial Generation**: Generates a new random serial number.
- **Execution**: Runs `volumeid.exe` with the selected drive and new serial.

## 📜 License
This project is open-source and available under the [MIT License](LICENSE).

## 🤝 Contribution
Feel free to submit issues or contribute to the project by making a pull request.

---
**Disclaimer:** This software is provided "as is" without any warranties. Use at your own risk.


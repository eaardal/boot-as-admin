# boot-as-admin
Boot any/several exe's as admin with only 1 login to UAC.

This is useful when you have enforced company group policies on your machine which prompts the UAC window with username and password for every. single. program. you want to run as Administrator.
By using this app you will enter UAC credentials _once_ and all processes started by this app will launch with Administrator privileges (if you have admin privileges on your machine, of course).  

1. Edit Bootlist.txt with your exe's.
2. Create a shortcut of BootAsAdmin.exe and put it on your desktop, start menu, or taskbar.
3. Right click on the shortcut -> Properties -> Advanced -> Check the Run as administrator box. OK all the way out.
4. Launch the shortcut, enter credentials, and all processes you listed should start.

### Using without building

- Use the "Download as ZIP" button on GitHub, then copy the contents of the `Dist` folder and run the .exe
- Or: browse into the `Dist` folder and download each file to your computer. Run the exe as is. 

Bootlist.txt example:

- Use \# to ignore/comment lines
- Any commandline arguments you normally use or can use when starting a new process will be passed along.

```
# Format: exe path ; arguments ; working dir

# Start PowerShell in the c:\git\myproject directory, run git status, and keep the window open (-NoExit) 
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe;-NoExit "git status";c:\git\myproject

# Start PowerShell in the c:\git\myproject directory, run gulp build, and close the window (no -NoExit switch)
C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe;"gulp build";c:\git\myproject

# Start Visual Studio with MyProject.sln
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe;c:\git\myproject\MyProject.sln

# Start Chrome on localhost:8080
C:\Program Files (x86)\Google\Chrome\Application\chrome.exe;http://localhost:8080

```

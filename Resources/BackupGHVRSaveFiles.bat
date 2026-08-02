@echo off

adb kill-server
adb start-server 

set folder="Desktop"

if exist %folder%\ (
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/SOA/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/Story/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/Survival/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/Settings/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/COOP_SOA/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/COOP_Story/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/COOP_Survival/ %folder%
  adb.exe pull /sdcard/Android/data/com.Incuvo.GreenHellVR/files/Challenge/ %folder%

  echo ----------------------------
  echo File saved at %folder% (if exists)
  
) else (
  echo /!\ Failed, Folder does not exist /!\
)
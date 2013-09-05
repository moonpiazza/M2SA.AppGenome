
sc create "MemCachedStats#M2SA.AppGenome.ServiceHost" binpath= "%~dp0\M2SA.AppGenome.ServiceHost.exe -service" displayname= "MemCachedStats#M2SA.AppGenome.ServiceHost" depend= Tcpip start= auto
sc description "MemCachedStats#M2SA.AppGenome.ServiceHost" "this is a mem-cached listener"
net start "MemCachedStats#M2SA.AppGenome.ServiceHost"
rem		net stop "MemCachedStats#M2SA.AppGenome.ServiceHost"
rem		sc delete "MemCachedStats#M2SA.AppGenome.ServiceHost"
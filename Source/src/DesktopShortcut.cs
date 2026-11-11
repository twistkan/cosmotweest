using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CosmoTweest {
// A portable launch creates its shortcut without an installer or administrator rights.
static class DesktopShortcut {
 static object Get(object item,string property){return item.GetType().InvokeMember(property,BindingFlags.GetProperty,null,item,null);}
 static void Set(object item,string property,object value){item.GetType().InvokeMember(property,BindingFlags.SetProperty,null,item,new[]{value});}
 static void Release(object item){if(item!=null&&Marshal.IsComObject(item))Marshal.FinalReleaseComObject(item);}
 public static string Ensure(string desktop,string executable){
  object shell=null,link=null;
  try {
   Directory.CreateDirectory(desktop);
   string path=Path.Combine(desktop,"CosmoTweest.lnk");
   executable=Path.GetFullPath(executable);
   shell=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
   link=shell.GetType().InvokeMember("CreateShortcut",BindingFlags.InvokeMethod,null,shell,new object[]{path});
   string target=Convert.ToString(Get(link,"TargetPath"));
   if(File.Exists(path)&&!String.Equals(Path.GetFileName(target),"CosmoTweest.exe",StringComparison.OrdinalIgnoreCase))
    return "The existing CosmoTweest desktop shortcut points to another program and was left unchanged.";
   string icon=executable+",0",working=Path.GetDirectoryName(executable);
   if(File.Exists(path)&&String.Equals(target,executable,StringComparison.OrdinalIgnoreCase)&&
      String.Equals(Convert.ToString(Get(link,"IconLocation")),icon,StringComparison.OrdinalIgnoreCase)&&
      String.Equals(Convert.ToString(Get(link,"WorkingDirectory")),working,StringComparison.OrdinalIgnoreCase))return null;
   Set(link,"TargetPath",executable);Set(link,"Arguments","");Set(link,"WorkingDirectory",working);
   Set(link,"IconLocation",icon);Set(link,"Description","CosmoTweest - five sectors, one rocket.");
   link.GetType().InvokeMember("Save",BindingFlags.InvokeMethod,null,link,null);
   return null;
  }catch(Exception e){return "The game can still run, but its desktop shortcut could not be created: "+e.Message;}
  finally{Release(link);Release(shell);}
 }
 public static void OnPortableLaunch(){
  // Respect the desktop-shortcut choice of players who used the optional installer.
  if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"cosmotweest.install")))return;
  string error=Ensure(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),Application.ExecutablePath);
  if(error!=null)MessageBox.Show(error,"CosmoTweest shortcut",MessageBoxButtons.OK,MessageBoxIcon.Information);
 }
 public static void Verify(string folder){
  Directory.CreateDirectory(folder);string exe=Application.ExecutablePath,path=Path.Combine(folder,"CosmoTweest.lnk");
  string error=Ensure(folder,exe);if(error!=null)throw new Exception(error);
  DateTime modified=File.GetLastWriteTimeUtc(path);error=Ensure(folder,exe);
  if(error!=null||modified!=File.GetLastWriteTimeUtc(path))throw new Exception("Shortcut creation is not idempotent.");
  object shell=null,link=null;
  try{shell=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));link=shell.GetType().InvokeMember("CreateShortcut",BindingFlags.InvokeMethod,null,shell,new object[]{path});
   if(!String.Equals(Convert.ToString(Get(link,"TargetPath")),exe,StringComparison.OrdinalIgnoreCase))throw new Exception("Shortcut target mismatch.");
   if(!String.Equals(Convert.ToString(Get(link,"IconLocation")),exe+",0",StringComparison.OrdinalIgnoreCase))throw new Exception("Shortcut icon mismatch.");
   if(!String.Equals(Convert.ToString(Get(link,"WorkingDirectory")),Path.GetDirectoryName(exe),StringComparison.OrdinalIgnoreCase))throw new Exception("Working directory mismatch.");
  }finally{Release(link);Release(shell);}
  File.WriteAllText(Path.Combine(folder,"shortcut-results.txt"),"PASS: shortcut target, embedded icon and working directory\r\nPASS: repeated launch leaves correct shortcut unchanged\r\n");
 }
}
}

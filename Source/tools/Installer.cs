using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
class Setup {
 static readonly string DefaultPath=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Programs","CosmoTweest");
 const string RegPath="Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\CosmoTweest";
 [STAThread] static void Main(string[] args){Application.EnableVisualStyles();try{
  if(args.Length==2&&args[0]=="--test-import"){TestProfileImport(Path.GetFullPath(args[1]));return;}
  if(args.Length==2&&args[0]=="--test-install"){Install(Path.GetFullPath(args[1]),true);return;}
  if(args.Length==2&&args[0]=="--test-remove"){Remove(Path.GetFullPath(args[1]),true);return;}
  if(args.Length==2&&args[0]=="--remove"){if(Path.GetFullPath(args[1])!=Path.GetFullPath(DefaultPath))throw new Exception("Unexpected installation path.");Remove(DefaultPath,false);MessageBox.Show("CosmoTweest was uninstalled. Your saved settings and scores were kept.","CosmoTweest");return;}
  if(args.Length>0&&args[0]=="--uninstall"){
   if(MessageBox.Show("Uninstall CosmoTweest? Saved scores and settings will be kept.","CosmoTweest",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;
   string temp=Path.Combine(Path.GetTempPath(),"CosmoTweest-Uninstall-"+Guid.NewGuid().ToString("N")+".exe");File.Copy(Application.ExecutablePath,temp);Process.Start(temp,"--remove \""+DefaultPath+"\"");return;
  }
  Form f=new Form{Text="CosmoTweest Setup",ClientSize=new Size(550,330),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,StartPosition=FormStartPosition.CenterScreen,BackColor=Color.FromArgb(12,22,40),ForeColor=Color.White};
  f.Controls.Add(new Label{Text="COSMOTWEEST",Font=new Font("Segoe UI",25),Location=new Point(28,26),Size=new Size(490,52)});
  f.Controls.Add(new Label{Text="Single-player space shooter - Fleet Command\n\nInstalls for your Windows account, with a desktop shortcut\nand an uninstaller. No administrator access required.\n\nLocation: "+DefaultPath,Font=new Font("Segoe UI",10),Location=new Point(30,90),Size=new Size(490,142)});
  CheckBox shortcut=new CheckBox{Text="Create desktop shortcut",Checked=true,Location=new Point(30,237),Size=new Size(260,26)};f.Controls.Add(shortcut);
  Button install=new Button{Text="Install",Location=new Point(325,276),Size=new Size(90,32),ForeColor=Color.Black};Button cancel=new Button{Text="Cancel",Location=new Point(425,276),Size=new Size(90,32),ForeColor=Color.Black};cancel.Click+=delegate{f.Close();};install.Click+=delegate{try{Install(DefaultPath,false,shortcut.Checked);MessageBox.Show("CosmoTweest is installed. Open it from your desktop or Start menu.","CosmoTweest Setup");f.Close();}catch(Exception e){MessageBox.Show(e.Message,"Installation failed");}};f.Controls.Add(install);f.Controls.Add(cancel);Application.Run(f);
 }catch(Exception e){try{File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"installer-error.log"),e.ToString());}catch{}Environment.ExitCode=1;MessageBox.Show(e.Message,"CosmoTweest Setup");}}
 static void ImportProfile(string source,string folder){string target=Path.Combine(folder,"settings.txt");if(File.Exists(source)&&!File.Exists(target)){Directory.CreateDirectory(folder);File.Copy(source,target,false);}}
 static void TestProfileImport(string root){Directory.CreateDirectory(root);string source=Path.Combine(root,"portable-settings.txt"),targetFolder=Path.Combine(root,"installed-profile");File.WriteAllText(source,"high=12345");ImportProfile(source,targetFolder);string target=Path.Combine(targetFolder,"settings.txt");if(File.ReadAllText(target)!="high=12345")throw new Exception("New-PC save import failed.");File.WriteAllText(source,"high=99999");ImportProfile(source,targetFolder);if(File.ReadAllText(target)!="high=12345")throw new Exception("Existing player save was overwritten.");File.WriteAllText(Path.Combine(root,"import-results.txt"),"PASS: setup imports portable progress when no local profile exists\r\nPASS: setup preserves an existing installed profile\r\n");}
 static void Link(string path,string target,string arguments){Type t=Type.GetTypeFromProgID("WScript.Shell");object shell=Activator.CreateInstance(t);object link=t.InvokeMember("CreateShortcut",BindingFlags.InvokeMethod,null,shell,new object[]{path});Type l=link.GetType();l.InvokeMember("TargetPath",BindingFlags.SetProperty,null,link,new object[]{target});l.InvokeMember("Arguments",BindingFlags.SetProperty,null,link,new object[]{arguments});l.InvokeMember("WorkingDirectory",BindingFlags.SetProperty,null,link,new object[]{Path.GetDirectoryName(target)});l.InvokeMember("IconLocation",BindingFlags.SetProperty,null,link,new object[]{target+",0"});l.InvokeMember("Save",BindingFlags.InvokeMethod,null,link,null);System.Runtime.InteropServices.Marshal.FinalReleaseComObject(link);System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);}
 static void Install(string path,bool test,bool desktop=true){Directory.CreateDirectory(path);using(Stream s=Assembly.GetExecutingAssembly().GetManifestResourceStream("GamePayload"))using(FileStream f=File.Create(Path.Combine(path,"CosmoTweest.exe")))s.CopyTo(f);File.Copy(Application.ExecutablePath,Path.Combine(path,"Uninstall.exe"),true);
  foreach(string name in new[]{"LICENSE.txt","CREDITS.md"}){using(Stream s=Assembly.GetExecutingAssembly().GetManifestResourceStream(name))using(FileStream f=File.Create(Path.Combine(path,name)))s.CopyTo(f);}
  File.WriteAllText(Path.Combine(path,"cosmotweest.install"),"CosmoTweest 1.0");
  if(test){Link(Path.Combine(path,"CosmoTweest.lnk"),Path.Combine(path,"CosmoTweest.exe"),"");return;}
  string portableSave=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data","settings.txt");string localFolder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CosmoTweest");string localSave=Path.Combine(localFolder,"settings.txt");ImportProfile(portableSave,localFolder);
  if(desktop)Link(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),"CosmoTweest.lnk"),Path.Combine(path,"CosmoTweest.exe"),"");
  string menu=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs),"CosmoTweest");Directory.CreateDirectory(menu);Link(Path.Combine(menu,"CosmoTweest.lnk"),Path.Combine(path,"CosmoTweest.exe"),"");Link(Path.Combine(menu,"Uninstall CosmoTweest.lnk"),Path.Combine(path,"Uninstall.exe"),"--uninstall");
  using(RegistryKey k=Registry.CurrentUser.CreateSubKey(RegPath)){k.SetValue("DisplayName","CosmoTweest");k.SetValue("DisplayVersion","1.1");k.SetValue("Publisher","CosmoTweest");k.SetValue("InstallLocation",path);k.SetValue("DisplayIcon",Path.Combine(path,"CosmoTweest.exe"));k.SetValue("UninstallString","\""+Path.Combine(path,"Uninstall.exe")+"\" --uninstall");k.SetValue("NoModify",1);k.SetValue("NoRepair",1);k.SetValue("EstimatedSize",(int)(new FileInfo(Path.Combine(path,"CosmoTweest.exe")).Length/1024*2));}
 }
 static void Remove(string path,bool test){if(!File.Exists(Path.Combine(path,"cosmotweest.install")))throw new Exception("Installation marker missing; no files removed.");foreach(Process p in Process.GetProcessesByName("CosmoTweest")){try{if(Path.GetFullPath(p.MainModule.FileName)==Path.Combine(path,"CosmoTweest.exe"))throw new Exception("Close CosmoTweest before uninstalling.");}catch(System.ComponentModel.Win32Exception){}}
  foreach(string name in new[]{"CosmoTweest.exe","Uninstall.exe","CosmoTweest.lnk","cosmotweest.install","LICENSE.txt","CREDITS.md"}){string file=Path.Combine(path,name);if(File.Exists(file))File.Delete(file);}if(Directory.GetFileSystemEntries(path).Length==0)Directory.Delete(path);
  if(test)return;string desktop=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),"CosmoTweest.lnk");if(File.Exists(desktop))File.Delete(desktop);string menu=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs),"CosmoTweest");foreach(string name in new[]{"CosmoTweest.lnk","Uninstall CosmoTweest.lnk"}){string f=Path.Combine(menu,name);if(File.Exists(f))File.Delete(f);}if(Directory.Exists(menu)&&Directory.GetFileSystemEntries(menu).Length==0)Directory.Delete(menu);Registry.CurrentUser.DeleteSubKeyTree(RegPath,false);
 }
}

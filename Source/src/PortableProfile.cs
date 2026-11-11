using System;
using System.IO;
namespace CosmoTweest {
static class PortableProfile {
 public static string Resolve(string gameFolder,string localFolder){
  string data=Path.Combine(gameFolder,"Data");
  return File.Exists(Path.Combine(data,"portable.mode"))?Path.Combine(data,"settings.txt"):Path.Combine(localFolder,"settings.txt");
 }
 // Invoked only by the relocation verification command, never by ordinary play.
 public static void Verify(string report){
  string expected=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data","settings.txt");
  if(!String.Equals(Settings.FilePath,expected,StringComparison.OrdinalIgnoreCase))throw new Exception("Portable profile did not follow the executable.");
  Settings before=Settings.Load(expected);before.Save(Settings.FilePath);Settings after=Settings.Load(expected);
  if(before.SaveFailed||after.High!=before.High||after.Unlocked!=before.Unlocked||after.Difficulty!=before.Difficulty||after.Muted!=before.Muted)throw new Exception("Portable progress/settings roundtrip failed.");
  File.WriteAllText(report,"PASS: relocated executable uses its own Data folder\r\nPASS: portable settings, score, unlocked levels, difficulty and mute survive save/load\r\nProfile: "+Settings.FilePath+"\r\n");
 }
}
}

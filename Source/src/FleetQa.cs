using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace CosmoTweest {
public sealed partial class Game {
 void FleetQaStep(int stage){switch(stage){
  case 26: world.Time=22;break;
  case 27: Entity mini=world.Entities.Find(delegate(Entity e){return e.Kind=="miniboss";});AssertQA(mini!=null&&world.Armored(mini),"live mini-boss appears with armored pods");mini.P=new V(480,220);foreach(Entity e in world.Entities)if(e.Owner==mini)e.P=mini.P+new V(e.Type*66,15);CaptureFrame("mini-boss");foreach(Entity e in world.Entities.ToArray())if(e.Kind=="minipart")world.Destroy(e);world.Destroy(mini);Entity drop=world.Entities.Find(delegate(Entity e){return e.Kind=="drone";});drop.P=world.Player;break;
  case 28: AssertQA(world.DroneCount==1&&world.MiniDefeated==1,"live mini reward equips drone");world.Add("enemy",new V(480,300),100,22);firing=true;break;
  case 29: AssertQA(world.DroneShots>0,"live drone fires with player");firing=false;world.Add("hostile",world.DronePosition(0),1,7);break;
  case 30: AssertQA(world.DroneBlocks>0,"live drone intercepts hostile shot");CaptureFrame("defence-drone");int tier=world.Tier,branch=world.Branch,drones=world.DroneCount,mode=world.Difficulty;Start(world.Level,tier,branch,drones,mode);AssertQA(world.Tier==tier&&world.Branch==branch&&world.DroneCount==drones,"retry keeps full ship path and drones");page="flightsettings";break;
  case 31: selected=0;KeyPressed(this,new KeyEventArgs(Keys.Enter));keys.Clear();AssertQA(settings.Difficulty==2&&Settings.Load(settingsPath).Difficulty==2&&world.Difficulty==1,"difficulty preference saves without changing active run");selected=2;KeyPressed(this,new KeyEventArgs(Keys.Enter));keys.Clear();AssertQA(settings.AutoFire&&Settings.Load(settingsPath).AutoFire,"autofire preference persists");CaptureFrame("flight-settings");Start(0);world.Transition=0;world.SpawnClock=999;world.SupplyClock=999;break;
  case 32: AssertQA(world.Difficulty==2&&world.Shots>0,"new run uses chosen difficulty and autofire");settings.Difficulty=0;settings.AutoFire=false;Save();Start(0);world.Transition=0;world.SpawnClock=999;world.SupplyClock=999;AssertQA(world.Difficulty==0&&world.Ammo==240&&world.Bombs==4,"relaxed mode starts with extra supplies");world.PendingTier=1;page="upgrade";selected=0;break;
  case 33: KeyPressed(this,new KeyEventArgs(Keys.D2));keys.Clear();AssertQA(page=="game"&&world.Branch==1&&world.Tier==1,"numeric key selects gunship path");world.Transition=0;firing=true;world.Add("enemy",new V(600,300),100,22);break;
  case 34: AssertQA(world.MissilesLaunched>0,"live gunship launches missiles");CaptureFrame("missile-gunship");settings.Difficulty=1;settings.AutoFire=false;Save();art.SaveFleetPreview(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"fleet-preview.png"));qaResults.Add("Audio output opened: "+audio.Available);qaResults.Add("Average render time: "+(renderMs/Math.Max(1,rendered)).ToString("F2")+"ms; frames="+rendered);File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ui-qa-results.txt"),qaResults);qa=false;Close();break;
 }}
}
}

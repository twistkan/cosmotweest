using System;
using System.IO;
namespace CosmoTweest {
static partial class Tests {
 static void RunFleetCampaignMatrix(string root){
  System.Collections.Generic.List<string> lines=new System.Collections.Generic.List<string>();
  for(int mode=0;mode<3;mode++)for(int branch=0;branch<3;branch++){
   World w=new World(0,100+mode*10+branch,mode);float elapsed=0;int repairs=0;
   while(!w.Won&&!w.Lost&&elapsed<1000){
    if(w.PendingTier>0)w.ChooseUpgrade(branch);
    Entity target=null;float best=float.MaxValue;foreach(Entity e in w.Entities){if(!w.Targetable(e)||e.P.Y<90)continue;float d=(e.P-w.Player).Length;if(d<best){best=d;target=e;}}
    Entity pickup=w.Entities.Find(delegate(Entity e){return !e.Dead&&(e.Kind=="upgrade"||e.Kind=="drone");})??w.Entities.Find(delegate(Entity e){return !e.Dead&&e.Kind=="pickup";});
    V goal=pickup!=null?pickup.P:new V(480+(float)Math.Sin(elapsed*.4)*220,650);
    V move=FlightControls.Follow(w.Player,goal,1f/60,430*w.FlightScale);
    w.Step(1f/60,move,target==null?new V(w.Player.X,0):target.P,true,false,430);
    if(w.Health<40&&!w.Lost){w.Health=100;repairs++;}elapsed+=1f/60;
   }
   Check(w.Won&&w.MiniDefeated==4&&w.Branch==branch&&w.Tier==3&&w.DroneCount==2,"full campaign "+World.Difficulties[mode]+" / "+World.Paths[branch]);
   lines.Add("PASS: "+World.Difficulties[mode]+" / "+World.Paths[branch]+" / VICTORY / "+elapsed.ToString("F1")+"s / minibosses="+w.MiniDefeated+" / hull rank="+w.Tier+" / drones="+w.DroneCount+" / blocks="+w.DroneBlocks+" / missiles="+w.MissilesLaunched+" / QA repairs="+repairs);
  }
  lines.Add("Each QA pilot repairs low hull; this tests progression/stability, not human difficulty balance.");
  File.WriteAllLines(Path.Combine(root,"fleet-campaign-matrix.txt"),lines);
 }
 static void RunFleetChecks(string root){
  World w=Fresh();w.PendingTier=1;float time=w.Time;w.Step(.1f,new V(1,0),new V(),true,false);Check(w.Time==time&&w.Tier==0,"simulation freezes until branch selection");Check(!w.ChooseUpgrade(9)&&w.PendingTier==1,"invalid branch rejected");Check(w.ChooseUpgrade(0)&&w.Tier==1&&w.Branch==0,"interceptor branch accepted");Check(w.FlightScale>1&&w.BoostDelay<3.2f,"interceptor agility advantage");
  w=Fresh();w.SetupLoadout(1,1,0);w.Add("enemy",new V(600,300),20,22);w.Step(.01f,new V(),new V(480,0),true,false);Entity m=w.Entities.Find(delegate(Entity e){return e.Kind=="missile";});Check(m!=null&&m.Vel.X>0&&w.MissilesLaunched==1,"gunship launches guided missile");Check(w.FlightScale<1,"gunship speed tradeoff");
  w=Fresh();Entity a=w.Add("enemy",new V(400,300),20,22),b=w.Add("enemy",new V(430,300),20,22);m=w.Add("missile",new V(410,300),6,8);w.MissileImpact(m);Check(a.HP==14&&b.HP==14&&m.Dead,"missile area damage");
  w=Fresh();w.SetupLoadout(1,2,0);float shield=w.Shield;w.Damage(20);Check(w.Health==100&&w.Shield==shield-20,"tank shield absorbs damage");for(int i=0;i<60;i++)w.Step(.1f,new V(),new V(480,0),false,false);Check(w.Shield>shield-20&&w.Shield<=w.MaxShield,"tank shield regenerates after delay");Check(w.FlightScale==.8f,"tank speed tradeoff");
  w.PendingTier=2;w.ChooseUpgrade(1);Check(w.MaxShield==0&&w.Shield==0&&w.Branch==1,"switching branch removes old shield");
  w=Fresh();w.Time=21.99f;w.Step(.02f,new V(),new V(),false,false);Entity mini=w.Entities.Find(delegate(Entity e){return e.Kind=="miniboss";});Check(mini!=null&&w.Entities.FindAll(delegate(Entity e){return e.Kind=="minipart";}).Count==2,"sector mini-boss has two destructible pods");float hp=mini.HP;w.Hit(mini,10);Check(Math.Abs(mini.HP-(hp-3))<.001f,"side pods armor mini-boss core");foreach(Entity part in w.Entities.ToArray())if(part.Kind=="minipart")w.Destroy(part);hp=mini.HP;w.Hit(mini,10);Check(mini.HP==hp-10&&!w.Armored(mini),"destroying pods exposes core");w.Destroy(mini);Entity drop=w.Entities.Find(delegate(Entity e){return e.Kind=="drone";});Check(w.MiniCleared&&w.MiniDefeated==1&&drop!=null,"mini-boss rewards drone core");w.CollectDrone(drop);Check(w.DroneCount==1,"drone pickup equips companion");w.Time=48;w.Step(.01f,new V(),new V(),false,false);Check(w.Level==1&&w.DroneCount==1&&!w.MiniSpawned,"sector transition retains drone and resets mini encounter");
  w=Fresh();w.DroneCount=1;Entity hostile=w.Add("hostile",w.DronePosition(0),1,7);w.Step(.01f,new V(),new V(),false,false);Check(hostile.Dead&&w.DroneBlocks==1&&w.DroneBlockClock>0,"defence drone intercepts projectile");hostile=w.Add("hostile",w.DronePosition(0),1,7);w.Step(.01f,new V(),new V(),false,false);Check(!hostile.Dead&&w.DroneBlocks==1,"drone interception has cooldown");w.Add("enemy",new V(480,300),10,22);w.Step(.01f,new V(),new V(480,0),true,false);Check(w.DroneShots>0&&w.Ammo==179,"drones shoot without extra player ammunition");w.DroneCount=2;w.Ammo=100;w.Health=40;w.CollectDrone(w.Add("drone",w.Player,1,20));Check(w.DroneCount==2&&w.Ammo==160&&w.Health==60,"drone cap converts excess cores to supplies");
  World easy=new World(0,7,0),normal=new World(0,7,1),hard=new World(0,7,2);Check(easy.Ammo>normal.Ammo&&normal.Ammo>hard.Ammo&&easy.Bombs>hard.Bombs,"difficulty starting resources");easy.Damage(20);normal.Damage(20);hard.Damage(20);Check(easy.Health>normal.Health&&normal.Health>hard.Health,"difficulty damage scales");Check(easy.EnemySpeed<normal.EnemySpeed&&hard.EnemySpeed>normal.EnemySpeed&&easy.SupplyInterval<hard.SupplyInterval,"difficulty speed and supply intervals");
  for(int mode=0;mode<3;mode++){w=new World(1,7,mode);w.Transition=0;w.SpawnClock=999;w.SupplyClock=999;w.Time=22;w.Step(.01f,new V(),new V(),false,false);Check(w.Entities.Exists(delegate(Entity e){return e.Kind=="miniboss"&&e.Type==1;}),"mini encounter on difficulty "+mode);}
  for(int mode=0;mode<3;mode++){w=new World(4,7,mode);w.Transition=0;w.SpawnClock=999;w.SupplyClock=999;w.Time=48;w.Step(.01f,new V(),new V(),false,false);Entity boss=w.Entities.Find(delegate(Entity e){return e.Kind=="boss";});boss.P=new V(480,190);boss.HP=boss.MaxHP*.49f;boss.Fire=0;w.Step(.01f,new V(),new V(),false,false);Entity shot=w.Entities.Find(delegate(Entity e){return e.Kind=="hostile";});Check(shot!=null&&Math.Abs(shot.Vel.Length-220*w.EnemySpeed)<.1f,"final boss overdrive uses half health on difficulty "+mode);}
  w=Fresh();w.SetupLoadout(3,2,2);Check(w.Tier==3&&w.Branch==2&&w.DroneCount==2&&w.Shield==85,"retry loadout restores path drones and shield");
  string path=Path.Combine(root,"fleet-test-settings.txt");Settings s=new Settings{Difficulty=2,AutoFire=true,High=100};s.RecordScore(0,2000);s.RecordScore(1,500);s.RecordScore(2,300);s.Save(path);Settings read=Settings.Load(path);Check(read.Difficulty==2&&read.AutoFire&&read.Best(0)==2000&&read.Best(1)==500&&read.Best(2)==300,"difficulty autofire and separate records persist");File.WriteAllText(path,"high=4321\nunlocked=4");read=Settings.Load(path);Check(read.Difficulty==1&&read.Best(1)==4321&&read.Unlocked==4,"old score migrates to standard without losing unlocks");File.Delete(path);
 }
}
}

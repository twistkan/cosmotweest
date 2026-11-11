using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace CosmoTweest {
public struct V {
 public float X,Y; public V(float x,float y){X=x;Y=y;} public float Length {get{return (float)Math.Sqrt(X*X+Y*Y);}}
 public V Unit {get{float l=Length;return l<.001f?new V(0,-1):this*(1/l);}}
 public static V operator +(V a,V b){return new V(a.X+b.X,a.Y+b.Y);} public static V operator -(V a,V b){return new V(a.X-b.X,a.Y-b.Y);} public static V operator *(V a,float s){return new V(a.X*s,a.Y*s);}
}
public sealed class Entity {
 public string Kind; public V P,Vel; public float HP,Radius,Age,Fire,Seed; public bool Dead; public int Type,Stage,Attacks;public Entity Owner;public float MaxHP;
 public Entity(string k,V p,float hp,float r){Kind=k;P=p;HP=hp;MaxHP=hp;Radius=r;}
}
public sealed class Particle {public V P,Vel;public float Life,Max;public int Color;public float Size;}
public sealed class Settings {
 public float Music=.32f,Effects=.55f,Master=.75f; public bool Fullscreen=false,Shake=true,MouseFlight=true,Muted=false; public int Unlocked=1,High=0,Width=1100,Height=800; public bool SaveFailed;public int Difficulty=1,BestRelaxed,BestStandard,BestExpert;public bool AutoFire=false;
 public float MusicGain {get{return Muted?0:Master*Music;}} public float EffectsGain {get{return Muted?0:Master*Effects;}}
 public int Best(int mode){return mode==0?BestRelaxed:mode==2?BestExpert:BestStandard;}
 public void RecordScore(int mode,int score){High=Math.Max(High,score);if(mode==0)BestRelaxed=Math.Max(BestRelaxed,score);else if(mode==2)BestExpert=Math.Max(BestExpert,score);else BestStandard=Math.Max(BestStandard,score);}
 public static string Folder {get{return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CosmoTweest");}}
 public static string FilePath {get{return PortableProfile.Resolve(AppDomain.CurrentDomain.BaseDirectory,Folder);}}
 public static Settings Load(string path){Settings s=new Settings();bool hasStandard=false;try {if(!File.Exists(path))return s;foreach(string line in File.ReadAllLines(path)){string[] a=line.Split('=');if(a.Length!=2)continue;float f;int n;if(a[0]=="difficulty"&&int.TryParse(a[1],out n))s.Difficulty=Math.Max(0,Math.Min(2,n));if(a[0]=="autofire")s.AutoFire=a[1]=="1";if(a[0]=="best0"&&int.TryParse(a[1],out n))s.BestRelaxed=Math.Max(0,n);if(a[0]=="best1"&&int.TryParse(a[1],out n)){s.BestStandard=Math.Max(0,n);hasStandard=true;}if(a[0]=="best2"&&int.TryParse(a[1],out n))s.BestExpert=Math.Max(0,n); if(a[0]=="master"&&float.TryParse(a[1],NumberStyles.Float,CultureInfo.InvariantCulture,out f))s.Master=Math.Max(0,Math.Min(1,f));if(a[0]=="muted")s.Muted=a[1]=="1";if(a[0]=="music"&&float.TryParse(a[1],NumberStyles.Float,CultureInfo.InvariantCulture,out f))s.Music=Math.Max(0,Math.Min(1,f));if(a[0]=="effects"&&float.TryParse(a[1],NumberStyles.Float,CultureInfo.InvariantCulture,out f))s.Effects=Math.Max(0,Math.Min(1,f));if(a[0]=="mouseflight")s.MouseFlight=a[1]!="0";if(a[0]=="fullscreen")s.Fullscreen=a[1]=="1";if(a[0]=="shake")s.Shake=a[1]=="1";if(a[0]=="unlocked"&&int.TryParse(a[1],out n))s.Unlocked=Math.Max(1,Math.Min(5,n));if(a[0]=="high"&&int.TryParse(a[1],out n))s.High=Math.Max(0,n);if(a[0]=="width"&&int.TryParse(a[1],out n))s.Width=Math.Max(800,Math.Min(2560,n));if(a[0]=="height"&&int.TryParse(a[1],out n))s.Height=Math.Max(600,Math.Min(1600,n));}if(!hasStandard)s.BestStandard=s.High;}catch{}return s;}
 public void Save(string path){try{Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));string tmp=path+".tmp";File.WriteAllLines(tmp,new[]{"difficulty="+Difficulty,"autofire="+(AutoFire?1:0),"best0="+BestRelaxed,"best1="+BestStandard,"best2="+BestExpert,"master="+Master.ToString(CultureInfo.InvariantCulture),"muted="+(Muted?1:0),"music="+Music.ToString(CultureInfo.InvariantCulture),"effects="+Effects.ToString(CultureInfo.InvariantCulture),"mouseflight="+(MouseFlight?1:0),"fullscreen="+(Fullscreen?1:0),"shake="+(Shake?1:0),"unlocked="+Unlocked,"high="+High,"width="+Width,"height="+Height});if(File.Exists(path)){try{File.Replace(tmp,path,null);}catch(IOException){File.Copy(tmp,path,true);File.Delete(tmp);}catch(UnauthorizedAccessException){File.Copy(tmp,path,true);File.Delete(tmp);}}else File.Move(tmp,path);SaveFailed=false;}catch{SaveFailed=true;}}
}
public sealed partial class World {
 public const float W=960,H=780,Duration=48;
 public static readonly string[] Names={"THE OUTER REACH","SHATTERED BELT","RAIDER CORRIDOR","ION TEMPEST","THE LAST SIGNAL"};
 public static readonly string[] Objectives={"Survive the patrol zone","Navigate the asteroid belt","Break through the raider fleet","Survive the crossfire","Destroy the dreadnought"};
 public static readonly string[] HullNames={"SCOUT","VANGUARD","TEMPEST","DREADSTAR"};
 public static readonly int[] UpgradeScores={1500,5000,11000};
 public static readonly string[] EnemyNames={"PATROLS","RAZOR WINGS","REAPERS","VOID STALKERS","DREAD GUARD"};
 public int Tier;public float UpgradeClock,NoticeTime;public string Notice="";
 public int NextUpgradeScore {get{return Tier<3?UpgradeScores[Tier]:0;}}
 public readonly List<Entity> Entities=new List<Entity>(); public readonly List<Particle> Particles=new List<Particle>();
 public readonly Random R; public V Player=new V(480,650),Aim=new V(480,0); public int Level,Score,Ammo=180,Bombs=3;public float Health=100,Time,Invulnerable,BoostCD,BoostTime,ShotCD,Shake,Flash,BombRing,Transition=3;public bool Won,Lost,PendingBoss;public float SpawnClock,SupplyClock=3,EmergencyClock;public int Kills,Shots,DamageEvents,Pickups; public Action<string> Sound;
 public World(int level,int seed,int difficulty=1){Level=level;R=new Random(seed);Difficulty=Math.Max(0,Math.Min(2,difficulty));if(Difficulty==0){Ammo=240;Bombs=4;}if(Difficulty==2){Ammo=160;Bombs=2;}}
 public float Rand(float a,float b){return a+(float)R.NextDouble()*(b-a);}
 public void Noise(string s){if(Sound!=null)Sound(s);}
 public void Burst(V p,int color,int count){for(int i=0;i<count;i++)Particles.Add(new Particle{P=p,Vel=new V(Rand(-170,170),Rand(-170,170)),Life=Rand(.25f,.7f),Max=.7f,Color=color,Size=Rand(2,6)});}
 public Entity Add(string kind,V p,float hp,float r){Entity e=new Entity(kind,p,hp,r);e.Seed=Rand(0,6.28f);if(kind=="enemy")e.Stage=Level;Entities.Add(e);return e;}
 public void Damage(float amount){if(Invulnerable>0||Lost||Won)return;amount*=Difficulty==0?.65f:Difficulty==2?1.3f:1;ShieldDelay=4;float absorbed=Math.Min(Shield,amount);Shield-=absorbed;amount-=absorbed;Health=Math.Max(0,Health-amount);Invulnerable=Difficulty==0?1.5f:Difficulty==2?.9f:1.15f;Shake=7;Flash=.25f;DamageEvents++;Noise("hurt");Burst(Player,1,18);if(Health<=0){Lost=true;Burst(Player,2,65);Noise("explode");}}
 public void Destroy(Entity e){if(e.Dead)return;if(DestroyFleetEntity(e))return;e.Dead=true;if(e.Kind=="crate"){Entity p=Add("pickup",e.P,1,17);p.Type=e.Type;p.Vel=new V(0,44);Burst(e.P,e.Type+3,15);Noise("crate");}else {Score+=e.Kind=="boss"?5000:e.Kind=="rock"?35:100+Level*25;Kills++;Burst(e.P,e.Kind=="rock"?2:1,e.Kind=="boss"?100:25);Noise("explode");Shake=Math.Max(Shake,e.Kind=="boss"?10:3);if(e.Kind=="boss"){Won=true;}}}
 public void Collect(Entity e){if(e.Type==0)Ammo=Math.Min(360,Ammo+100);else if(e.Type==1)Health=Math.Min(100,Health+30);else Bombs=Math.Min(6,Bombs+1);e.Dead=true;Pickups++;Score+=25;Burst(e.P,e.Type+3,25);Noise("pickup");}
 public void OfferUpgrade(float dt){
  UpgradeClock=Math.Max(0,UpgradeClock-dt);
  if(PendingTier>0||Tier>=3||Score<NextUpgradeScore||UpgradeClock>0||Won||Lost||Transition>0)return;
  if(Entities.Exists(delegate(Entity e){return !e.Dead&&e.Kind=="upgrade";}))return;
  Entity core=Add("upgrade",new V(Math.Max(90,Math.Min(870,Player.X+Rand(-80,80))),Math.Max(140,Math.Min(430,Player.Y-170))),1,25);
  core.Type=Tier+1;core.Vel=new V(0,32);UpgradeClock=15;Notice="UPGRADE CORE / RANK "+core.Type+" / COLLECT TO CHOOSE A SHIP";NoticeTime=5;Noise("pickup");
 }
 public void CollectUpgrade(Entity e){
  if(e.Dead||e.Kind!="upgrade")return;e.Dead=true;
  if(e.Type!=Tier+1||Tier>=3)return;
  PendingTier=e.Type;Notice="UPGRADE CORE RECOVERED / CHOOSE YOUR FLIGHT PATH";NoticeTime=5;
 }
 public void FireVolley(){
  Ammo--;Shots++;ShotCD=(Tier==3?.14f:.115f)*(Tier>0&&Branch==0?.85f:Tier>0&&Branch==1?1.15f:1);if(Tier>0&&Branch==1&&MissileClock<=0)FireMissile();V d=(Aim-Player).Unit;V side=new V(-d.Y,d.X);
  int count=Tier==0?1:Tier==1?2:Tier==2?3:5;
  for(int i=0;i<count;i++){float offset=i-(count-1)/2f;double angle=Tier>=2?offset*.105:0;
   V direction=new V(d.X*(float)Math.Cos(angle)-d.Y*(float)Math.Sin(angle),d.X*(float)Math.Sin(angle)+d.Y*(float)Math.Cos(angle));
   Entity b=Add("bullet",Player+d*26+side*(offset*(Tier==1?18:9)),Tier==3?1.5f:1,5);b.Type=Tier;b.Vel=direction*(Tier==3?800:720);
  }Noise("shot");
 }
 public void Bomb(){if(Bombs<=0||Transition>0||Lost||Won)return;Bombs--;BombRing=.8f;Shake=8;Noise("bomb");List<Entity> targets=new List<Entity>(Entities);foreach(Entity e in targets){if(e.Kind=="hostile"){e.Dead=true;continue;}if((e.P-Player).Length<310&&(e.Kind=="enemy"||e.Kind=="rock"||e.Kind=="crate"||e.Kind=="boss"||e.Kind=="miniboss"||e.Kind=="minipart")){Hit(e,e.Kind=="boss"||e.Kind=="miniboss"?65:e.Kind=="minipart"?25:8);}}}
 public void SpawnSupply(bool emergency){Entity c=Add("crate",new V(emergency?Player.X:Rand(100,860),emergency?Math.Max(120,Player.Y-170):-35),1,23);c.Type=emergency?0:(R.NextDouble()<.54?0:R.NextDouble()<.75?1:2);c.Vel=new V(0,emergency?25:62);}
 public void Step(float dt,V move,V aim,bool fire,bool boost,float moveSpeed=290,bool upwardFire=false){
  if(Lost||Won){Effects(dt);return;}if(PendingTier>0)return; Aim=aim;Effects(dt);if(Transition>0){Transition-=dt;return;}
  Invulnerable=Math.Max(0,Invulnerable-dt);BoostCD=Math.Max(0,BoostCD-dt);BoostTime=Math.Max(0,BoostTime-dt);ShotCD-=dt;
  if(boost&&BoostCD<=0){BoostCD=BoostDelay;BoostTime=.2f;Invulnerable=Math.Max(Invulnerable,.28f);Noise("boost");}
  if(move.Length>1)move=move.Unit;Player+=move*(dt*(BoostTime>0?900:moveSpeed*FlightScale));Player.X=Math.Max(24,Math.Min(W-24,Player.X));Player.Y=Math.Max(105,Math.Min(H-28,Player.Y));if(upwardFire)Aim=new V(Player.X,-1000);
  if(R.NextDouble()<.7)Particles.Add(new Particle{P=Player+new V(Rand(-7,7),20),Vel=new V(Rand(-16,16),BoostTime>0?260:110),Life=.25f,Max=.25f,Color=0,Size=BoostTime>0?9:4});
  if(fire&&ShotCD<=0&&Ammo>0)FireVolley();
  Time+=dt;SpawnClock-=dt;SupplyClock-=dt;EmergencyClock-=dt;
  if(SupplyClock<=0){SpawnSupply(false);SupplyClock=SupplyInterval;}
  if(Ammo<20&&EmergencyClock<=0){SpawnSupply(true);EmergencyClock=6;}
  // An empty magazine can always crack supply containers with the free rescue pulse.
  if(Ammo==0&&fire&&ShotCD<=0){ShotCD=.65f;Entity b=Add("rescue",Player,1,7);b.Vel=(Aim-Player).Unit*520;Noise("shot");}
  if(Level<4&&Time>=22&&Time<Duration&&!MiniSpawned)SpawnMini();
  if(Time<Duration&&SpawnClock<=0&&!Entities.Exists(delegate(Entity active){return !active.Dead&&active.Kind=="miniboss";})){SpawnClock=(1.4f-Level*.16f)*SpawnInterval;Entity e;
   if(R.NextDouble()<.23+Level*.045){e=Add("rock",new V(Rand(40,920),-40),3, Rand(19,36));e.Vel=new V(Rand(-24,24),(Rand(85,115)+Level*14)*EnemySpeed);}
   else{int count=Level>=2&&R.NextDouble()<.36?3:1;float x=Rand(130,830);for(int i=0;i<count;i++){e=Add("enemy",new V(Math.Max(40,Math.Min(920,x+(i-(count-1)/2f)*85)),-40-i*28),Level>=3?4:3,22);e.Type=R.Next(Math.Min(4,Level+1));e.Vel=new V(0,(82+Level*20)*EnemySpeed);e.Fire=1.1f;}}
  }
  if(Level==4&&Time>=Duration&&!PendingBoss){PendingBoss=true;Add("boss",new V(480,-100),680*BossScale,85).Fire=2;Noise("bomb");}
  StepFleet(dt,fire);List<Entity> frame=new List<Entity>(Entities);
  foreach(Entity e in frame){if(e.Dead)continue;e.Age+=dt;
   if(MoveFleetEntity(e,dt)){}else if(e.Kind=="enemy") {float dx=e.Type==1?(float)Math.Cos(e.Age*2.8+e.Seed)*110:e.Type==2?(Player.X-e.P.X)*.42f:e.Type==3?(float)Math.Sin(e.Age*4)*155:0;e.P+=new V(dx,e.Vel.Y)*dt;e.Fire-=dt;if(e.Fire<=0&&e.P.Y>40&&e.P.Y<580){e.Fire=2.3f-Level*.21f;V d=(Player-e.P).Unit;int n=Level>=2?3:1;for(int i=0;i<n;i++){double a=(i-(n-1)/2f)*.22;V v=new V(d.X*(float)Math.Cos(a)-d.Y*(float)Math.Sin(a),d.X*(float)Math.Sin(a)+d.Y*(float)Math.Cos(a));Add("hostile",e.P,1,7).Vel=v*(140+Level*18)*EnemySpeed;}}}
   else if(e.Kind=="boss"){e.P=new V(480+(float)Math.Sin(e.Age*.65)*245,Math.Min(190,e.P.Y+70*dt));e.Fire-=dt;if(e.Fire<=0){bool phase=e.HP<e.MaxHP/2;e.Fire=phase?.58f:.95f;V d=(Player-e.P).Unit;for(int i=-3;i<=3;i++){double a=i*(phase?.24:.17);Add("hostile",e.P+new V(0,48),1,8).Vel=new V(d.X*(float)Math.Cos(a)-d.Y*(float)Math.Sin(a),d.X*(float)Math.Sin(a)+d.Y*(float)Math.Cos(a))*(phase?220:175)*EnemySpeed;}if(phase&&R.NextDouble()<.5){Entity r=Add("rock",new V(Rand(40,920),-40),2,22);r.Vel=new V(0,180);}}}
   else e.P+=e.Vel*dt;
   if(e.P.Y>H+70||e.P.Y< -180||e.P.X< -100||e.P.X>W+100||((e.Kind=="bullet"||e.Kind=="rescue")&&e.Age>2))e.Dead=true;
   if((e.Kind=="enemy"||e.Kind=="rock"||e.Kind=="boss"||e.Kind=="miniboss"||e.Kind=="minipart"||e.Kind=="hostile")&&(e.P-Player).Length<e.Radius+15){Damage(e.Kind=="hostile"?12:22);if(e.Kind=="hostile")e.Dead=true;}
   if(e.Kind=="drone"&&(e.P-Player).Length<40)CollectDrone(e);
   if(e.Kind=="pickup"&&(e.P-Player).Length<40)Collect(e);
   if(e.Kind=="upgrade"&&(e.P-Player).Length<40)CollectUpgrade(e);
   // Containers can also be rammed safely, so supplies never require ammunition.
   if(e.Kind=="crate"&&(e.P-Player).Length<39){Destroy(e);}
  }
  foreach(Entity b in frame){if(b.Dead||(b.Kind!="bullet"&&b.Kind!="rescue"&&b.Kind!="missile"))continue;foreach(Entity e in frame){if(e.Dead||!Targetable(e)||(b.Kind=="rescue"&&e.Kind!="crate"))continue;if((b.P-e.P).Length<e.Radius+b.Radius){if(b.Kind=="missile")MissileImpact(b);else{b.Dead=true;Hit(e,b.HP);}break;}}}
  Entities.RemoveAll(delegate(Entity e){return e.Dead;});
  OfferUpgrade(dt);
  if(Level<4&&Time>=Duration&&Entities.Find(delegate(Entity e){return e.Kind=="enemy"||e.Kind=="rock"||e.Kind=="miniboss"||e.Kind=="minipart";})==null){Level++;Time=0;Transition=3.5f;MiniSpawned=false;MiniCleared=false;Entities.RemoveAll(delegate(Entity item){return item.Kind!="pickup"&&item.Kind!="drone"&&item.Kind!="upgrade";});Ammo=Math.Min(360,Ammo+100);Health=Math.Min(100,Health+25);Bombs=Math.Min(6,Bombs+1);SupplyClock=2;SpawnClock=0;Noise("pickup");}
 }
 public void Effects(float dt){NoticeTime=Math.Max(0,NoticeTime-dt);Shake=Math.Max(0,Shake-dt*25);Flash=Math.Max(0,Flash-dt);BombRing=Math.Max(0,BombRing-dt);for(int i=Particles.Count-1;i>=0;i--){Particle p=Particles[i];p.Life-=dt;p.P+=p.Vel*dt;if(p.Life<=0)Particles.RemoveAt(i);}if(Particles.Count>1400)Particles.RemoveRange(0,Particles.Count-1400);}
}
}

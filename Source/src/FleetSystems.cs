using System;
using System.Collections.Generic;
namespace CosmoTweest {
public sealed partial class World {
 public static readonly string[] Difficulties={"RELAXED","STANDARD","EXPERT"};
 public static readonly string[] Paths={"INTERCEPTOR","MISSILE GUNSHIP","SHIELD TANK"};
 public static readonly string[] MiniNames={"RAIDER ACE","BELT HARVESTER","BROOD CARRIER","ION WARDEN"};
 public readonly int Difficulty;public int Branch=-1,PendingTier,DroneCount,MiniDefeated,MissilesLaunched,DroneBlocks,DroneShots;
 public bool MiniSpawned,MiniCleared;public float Shield,ShieldDelay,MissileClock,DroneFireClock,DroneBlockClock;
 public float EnemySpeed {get{return Difficulty==0?.78f:Difficulty==2?1.22f:1;}}
 public float SpawnInterval {get{return Difficulty==0?1.25f:Difficulty==2?.8f:1;}}
 public float SupplyInterval {get{return Difficulty==0?4.2f:Difficulty==2?6.5f:5.5f;}}
 public float BossScale {get{return Difficulty==0?.85f:Difficulty==2?1.18f:1;}}
 public float FlightScale {get{return Tier==0?1:Branch==0?1.2f:Branch==1?.9f:Branch==2?.8f:1;}}
 public float BoostDelay {get{return Tier>0&&Branch==0?2.2f:3.2f;}}
 public float MaxShield {get{return Tier>0&&Branch==2?40+Tier*15:0;}}
 public string PlayerName {get{return Tier==0?"SCOUT":Branch<0?HullNames[Tier]:Paths[Branch]+" "+new[]{"","I","II","III"}[Tier];}}
 public string PlayerSprite {get{return Tier==0||Branch<0?"player"+Tier:"path"+Branch+"_"+Tier;}}
 public void SetupLoadout(int tier,int branch,int drones){Tier=Math.Max(0,Math.Min(3,tier));Branch=Tier==0?-1:Math.Max(-1,Math.Min(2,branch));DroneCount=Math.Max(0,Math.Min(2,drones));Shield=MaxShield;}
 public bool ChooseUpgrade(int branch){
  if(PendingTier!=Tier+1||PendingTier>3||branch<0||branch>2)return false;
  Tier=PendingTier;PendingTier=0;Branch=branch;Shield=MaxShield;ShieldDelay=0;
  Ammo=Math.Min(360,Ammo+80);Health=Math.Min(100,Health+15);Invulnerable=Math.Max(Invulnerable,1.5f);UpgradeClock=15;
  Notice=PlayerName+" ONLINE / "+(branch==0?"FAST CANNONS + AGILITY":branch==1?"HOMING MISSILES ARMED":"REGENERATING SHIELD ACTIVE");NoticeTime=5;
  Burst(Player,2,70);Noise("upgrade");Shake=4;return true;
 }
 public bool Targetable(Entity e){return !e.Dead&&(e.Kind=="enemy"||e.Kind=="boss"||e.Kind=="miniboss"||e.Kind=="minipart"||e.Kind=="rock"||e.Kind=="crate");}
 public bool Armored(Entity e){return e.Kind=="miniboss"&&Entities.Exists(delegate(Entity p){return !p.Dead&&p.Kind=="minipart"&&p.Owner==e;});}
 public void Hit(Entity e,float amount){if(e.Dead)return;e.HP-=amount*(Armored(e)?.3f:1);if(e.HP<=0)Destroy(e);else Burst(e.P,0,3);}
 public V DronePosition(int index){double angle=Time*1.8+index*Math.PI;return Player+new V((float)Math.Cos(angle)*46,(float)Math.Sin(angle)*28-8);}
 public void CollectDrone(Entity e){if(e.Dead)return;e.Dead=true;if(DroneCount<2){DroneCount++;Notice="DEFENCE DRONE ONLINE / "+DroneCount+" OF 2";}else{Ammo=Math.Min(360,Ammo+60);Health=Math.Min(100,Health+20);Notice="DRONE BAY FULL / SUPPLIES RECOVERED";}NoticeTime=4;Burst(Player,0,25);Noise("upgrade");}
 public bool DestroyFleetEntity(Entity e){
  if(e.Kind!="miniboss"&&e.Kind!="minipart")return false;
  e.Dead=true;Kills++;Score+=e.Kind=="miniboss"?1000+e.Type*250:150;Burst(e.P,2,e.Kind=="miniboss"?75:25);Noise("explode");
  if(e.Kind=="miniboss"){
   MiniCleared=true;MiniDefeated++;foreach(Entity p in Entities)if(p.Owner==e)p.Dead=true;
   Entity drop=Add("drone",e.P,1,23);drop.Vel=new V(0,34);Ammo=Math.Min(360,Ammo+40);
   Notice=MiniNames[e.Type]+" DEFEATED / COLLECT THE CYAN DRONE CORE";NoticeTime=5;Shake=6;
  }return true;
 }
 public void SpawnMini(){
  if(Level>=4||MiniSpawned)return;MiniSpawned=true;Entity m=Add("miniboss",new V(480,-100),(100+Level*45)*BossScale,58);m.Type=Level;m.Fire=2.8f;
  for(int side=-1;side<=1;side+=2){Entity p=Add("minipart",m.P+new V(side*66,15),(24+Level*8)*BossScale,20);p.Owner=m;p.Type=side;}
  Notice=MiniNames[Level]+" / DESTROY THE GLOWING SIDE PODS";NoticeTime=5;Noise("bomb");
 }
 void MiniAttack(Entity e){
  V aim=(Player-e.P).Unit;int stage=e.Type;e.Attacks++;
  if(stage==0||stage==2){for(int i=-1;i<=1;i++){double a=i*.2;Add("hostile",e.P+new V(0,45),1,7).Vel=new V(aim.X*(float)Math.Cos(a)-aim.Y*(float)Math.Sin(a),aim.X*(float)Math.Sin(a)+aim.Y*(float)Math.Cos(a))*(165+stage*15)*EnemySpeed;}}
  else{int count=stage==1?8:11;for(int i=0;i<count;i++){double a=i*Math.PI*2/count+e.Age*.15;Add("hostile",e.P,1,7).Vel=new V((float)Math.Cos(a),(float)Math.Sin(a))*(135+stage*12)*EnemySpeed;}}
  if(stage==1&&e.Attacks%2==0){Entity r=Add("rock",new V(Rand(90,870),-30),3,25);r.Vel=new V(0,150*EnemySpeed);}
  if(stage==2&&e.Attacks%2==0){for(int i=-1;i<=1;i+=2){Entity escort=Add("enemy",e.P+new V(i*90,30),3,22);escort.Type=1;escort.Vel=new V(0,120*EnemySpeed);escort.Fire=1.5f;}}
  if(stage==3){Add("hostile",e.P+new V(0,45),1,9).Vel=aim*240*EnemySpeed;}
 }
 public bool MoveFleetEntity(Entity e,float dt){
  if(e.Kind=="miniboss"){e.P=new V(480+(float)Math.Sin(e.Age*(.65f+e.Type*.1f))*210,Math.Min(220,e.P.Y+95*dt));e.Fire-=dt;if(e.Fire<=0&&e.P.Y>120){e.Fire=(e.Type==0?.9f:e.Type==1?1.5f:1.25f)/EnemySpeed;MiniAttack(e);}return true;}
  if(e.Kind=="minipart"){if(e.Owner==null||e.Owner.Dead)e.Dead=true;else e.P=e.Owner.P+new V(e.Type*66,15);return true;}
  if(e.Kind=="missile"){
   Entity nearest=null;float best=700;foreach(Entity t in Entities){if(!Targetable(t)||t.Kind=="crate"||t.P.Y<90)continue;float distance=(t.P-e.P).Length;if(distance<best){best=distance;nearest=t;}}
   if(nearest!=null){V desired=(nearest.P-e.P).Unit*420;e.Vel=e.Vel*Math.Max(0,1-dt*5)+desired*Math.Min(1,dt*5);}e.P+=e.Vel*dt;if(e.Age>4)e.Dead=true;return true;
  }return false;
 }
 public void FireMissile(){
  int count=Tier==3?2:1;for(int i=0;i<count;i++){Entity m=Add("missile",Player+new V(count==1?0:(i==0?-20:20),-25),4+Tier,8);m.Vel=(Aim-Player).Unit*280;MissilesLaunched++;}MissileClock=1.25f-Tier*.15f;
 }
 public void StepFleet(float dt,bool firing){
  MissileClock=Math.Max(0,MissileClock-dt);ShieldDelay=Math.Max(0,ShieldDelay-dt);if(ShieldDelay<=0)Shield=Math.Min(MaxShield,Shield+dt*9);
  DroneBlockClock=Math.Max(0,DroneBlockClock-dt);DroneFireClock-=dt;
  if(DroneCount>0&&firing&&DroneFireClock<=0){
   DroneFireClock=.85f;for(int i=0;i<DroneCount;i++){V pos=DronePosition(i);Entity target=null;float best=600;
    foreach(Entity e in Entities){if(!Targetable(e)||e.Kind=="crate"||e.P.Y<90)continue;float distance=(e.P-pos).Length;if(distance<best){best=distance;target=e;}}
    if(target!=null){Entity b=Add("bullet",pos,.8f,4);b.Vel=(target.P-pos).Unit*650;b.Type=4;DroneShots++;}
   }
  }
  if(DroneBlockClock<=0&&DroneCount>0){foreach(Entity e in Entities){if(e.Dead||e.Kind!="hostile")continue;bool blocked=false;for(int i=0;i<DroneCount;i++)if((e.P-DronePosition(i)).Length<23){blocked=true;break;}
    if(blocked){e.Dead=true;DroneBlockClock=DroneCount==2?1:1.4f;DroneBlocks++;Burst(e.P,4,10);Noise("boost");break;}}
  }
 }
 public void MissileImpact(Entity missile){missile.Dead=true;Burst(missile.P,2,22);List<Entity> nearby=new List<Entity>(Entities);foreach(Entity e in nearby)if(Targetable(e)&&(e.P-missile.P).Length<85+e.Radius)Hit(e,missile.HP);Noise("explode");}
}
}

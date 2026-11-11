using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace CosmoTweest {
public sealed partial class Art {
 public static Color PathColor(int branch){return branch==0?Cyan:branch==1?Gold:Green;}
 void InitFleetArt(){
  for(int branch=0;branch<3;branch++)for(int rank=1;rank<=3;rank++)Sprites["path"+branch+"_"+rank]=PathShip(branch,rank);
  for(int stage=0;stage<4;stage++)Sprites["mini"+stage]=MiniShip(stage);
  Bitmap drone=new Bitmap(44,44);using(Graphics g=Graphics.FromImage(drone)){g.SmoothingMode=SmoothingMode.AntiAlias;Poly(g,Color.SlateGray,new float[]{22,2,37,12,41,30,22,42,3,30,7,12});Poly(g,Color.FromArgb(23,51,67),new float[]{22,7,34,15,34,28,22,36,10,28,10,15});using(Pen p=new Pen(Cyan,2))g.DrawEllipse(p,14,14,16,16);g.FillEllipse(Brushes.White,19,19,6,6);}Sprites["drone"]=drone;
 }
 static Bitmap PathShip(int branch,int tier){Bitmap b=new Bitmap(160,160);using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;
  Color c=PathColor(branch);if(branch==2){Poly(g,Color.FromArgb(52,87,90),new float[]{80,13,122,39,145,112,129,147,96,130,80,147,64,130,31,147,15,112,38,39});using(Pen p=new Pen(Green,3))g.DrawPolygon(p,Points(new float[]{80,12,133,49,146,119,118,147,42,147,14,119,27,49}));}
  using(Bitmap core=EvolvedPlayer(tier))g.DrawImage(core,branch==0?28:16,9,branch==0?104:128,144);
  if(branch==0){Poly(g,Cyan,new float[]{39,70,12,132,38,118,51,88});Poly(g,Cyan,new float[]{121,70,148,132,122,118,109,88});}
  if(branch==1){for(int side=-1;side<=1;side+=2){float x=80+side*47;g.FillRectangle(Brushes.DarkSlateGray,x-11,48,22,65);using(Pen p=new Pen(Gold,2))g.DrawRectangle(p,x-11,48,22,65);for(int i=0;i<3;i++){g.FillRectangle(Brushes.LightSteelBlue,x-6,54+i*17,12,10);g.FillRectangle(Brushes.Orange,x-3,54+i*17,6,4);}}}
  using(Pen p=new Pen(c,3))g.DrawEllipse(p,70,82,20,20);
 }return b;}
 static Bitmap MiniShip(int stage){Bitmap b=new Bitmap(180,150);using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;
  float[] shape=stage==0?new float[]{90,9,118,49,175,31,154,123,117,104,90,147,63,104,26,123,5,31,62,49}:
   stage==1?new float[]{30,12,65,29,115,29,150,12,174,49,153,128,123,112,90,146,57,112,27,128,6,49}:
   stage==2?new float[]{90,6,149,24,176,76,154,133,119,112,90,147,61,112,26,133,4,76,31,24}:
   new float[]{90,2,119,29,165,10,153,69,178,96,140,145,112,120,90,148,68,120,40,145,2,96,27,69,15,10,61,29};
  using(LinearGradientBrush fill=new LinearGradientBrush(new Point(20,10),new Point(150,140),Color.FromArgb(134,132,160),Color.FromArgb(33,34,60)))g.FillPolygon(fill,Points(shape));
  using(Pen p=new Pen(Pink,2))g.DrawPolygon(p,Points(shape));Poly(g,Color.FromArgb(20,27,47),new float[]{90,19,115,66,109,108,90,130,71,108,65,66});
  using(Pen p=new Pen(stage==3?Color.MediumOrchid:Gold,4))g.DrawEllipse(p,75,67,30,30);g.FillEllipse(Brushes.White,85,77,10,10);
  for(int side=-1;side<=1;side+=2){using(Pen p=new Pen(Pink,3))g.DrawLine(p,90+side*37,50,90+side*48,109);}
  if(stage==1){using(Pen p=new Pen(Gold,7)){g.DrawLine(p,29,65,22,111);g.DrawLine(p,151,65,158,111);}}
  if(stage==2){g.FillRectangle(Brushes.Black,36,56,22,39);g.FillRectangle(Brushes.Black,122,56,22,39);}
 }return b;}
 bool DrawFleetEntity(Graphics g,Entity e,float time){
  if(e.Kind=="miniboss"){Sprite(g,Sprites["mini"+e.Type],e.P,146,122,0);return true;}
  if(e.Kind=="minipart"){using(Brush b=new SolidBrush(Color.FromArgb(38,29,52)))g.FillEllipse(b,e.P.X-19,e.P.Y-24,38,48);using(Pen p=new Pen(Pink,3))g.DrawEllipse(p,e.P.X-16,e.P.Y-21,32,42);using(Pen p=new Pen(Gold,4))g.DrawEllipse(p,e.P.X-8,e.P.Y-10,16,20);using(Brush b=new SolidBrush(Color.White))g.FillEllipse(b,e.P.X-3,e.P.Y-4,6,8);using(Brush b=new SolidBrush(Pink))g.FillRectangle(b,e.P.X-20,e.P.Y-31,40*Math.Max(0,e.HP/e.MaxHP),3);return true;}
  if(e.Kind=="drone"){using(Pen p=new Pen(Cyan,2))g.DrawEllipse(p,e.P.X-28,e.P.Y-28,56,56);Sprite(g,Sprites["drone"],e.P,34,34,time*35);Text(g,"DRONE",e.P.X,e.P.Y+34,14,Cyan,true);return true;}
  if(e.Kind=="missile"){float angle=(float)(Math.Atan2(e.Vel.X,-e.Vel.Y)*180/Math.PI);GraphicsState state=g.Save();g.TranslateTransform(e.P.X,e.P.Y);g.RotateTransform(angle);g.FillEllipse(Brushes.Orange,-4,4,8,15);Poly(g,Color.LightSteelBlue,new float[]{0,-14,-5,-3,-5,8,5,8,5,-3});Poly(g,Gold,new float[]{0,-16,-5,-5,5,-5});g.Restore(state);return true;}
  return false;
 }
 void DrawFleetCompanions(Graphics g,World w,float time){
  if(w.MaxShield>0&&w.Shield>0){using(Pen p=new Pen(Color.FromArgb((int)(45+120*w.Shield/w.MaxShield),Green),2))g.DrawEllipse(p,w.Player.X-43,w.Player.Y-43,86,86);}
  if(w.Lost)return;for(int i=0;i<w.DroneCount;i++){V pos=w.DronePosition(i);Sprite(g,Sprites["drone"],pos,25,25,time*35);if(w.DroneBlockClock<=0)using(Pen p=new Pen(Color.FromArgb(110,Cyan),1))g.DrawEllipse(p,pos.X-17,pos.Y-17,34,34);}
 }
 public void SaveFleetPreview(string path){using(Bitmap b=new Bitmap(1200,800))using(Graphics g=Graphics.FromImage(b)){g.Clear(Color.FromArgb(7,13,29));g.SmoothingMode=SmoothingMode.AntiAlias;Text(g,"COSMOTWEEST / FLEET COMMAND",600,18,30,Color.White,true);
  string[][] notes={new[]{"Fast flight + rapid cannons","2.2-second boost cooldown"},new[]{"Homing missiles + area damage","Slower flight / heavier firepower"},new[]{"Regenerating shield","Heavy armor / slower flight"}};
  for(int branch=0;branch<3;branch++){float x=200+branch*400;Sprite(g,Sprites["path"+branch+"_3"],new V(x,180),132,132,0);Text(g,World.Paths[branch],x,267,22,PathColor(branch),true);Text(g,notes[branch][0],x,309,16,Color.White,true);Text(g,notes[branch][1],x,336,16,Color.LightSteelBlue,true);}
  Text(g,"SECTOR MINI-BOSSES / DESTROY SIDE PODS TO BREAK ARMOR",600,402,20,Gold,true);
  for(int stage=0;stage<4;stage++){float x=150+stage*300;Sprite(g,Sprites["mini"+stage],new V(x,520),135,113,0);Text(g,World.MiniNames[stage],x,594,18,Pink,true);}
  Sprite(g,Sprites["drone"],new V(320,700),46,46,0);Text(g,"DEFENCE DRONES / AUTO-FIRE + PROJECTILE INTERCEPTION",650,690,19,Cyan,true);Text(g,"RELAXED / STANDARD / EXPERT + OPTIONAL AUTO-FIRE",600,756,18,Color.LightSteelBlue,true);b.Save(path);
 }}
}
}

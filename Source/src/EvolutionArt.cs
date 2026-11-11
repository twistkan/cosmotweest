using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace CosmoTweest {
public sealed partial class Art {
 static Bitmap EvolvedPlayer(int tier){
  if(tier==0)return Ship(true,0);
  Bitmap b=new Bitmap(128,144,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
  using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;
   Color light=tier==3?Gold:tier==2?Color.FromArgb(176,164,255):Cyan;
   Poly(g,Color.FromArgb(19,31,51),new float[]{64,3,82,40,119,104,111,127,85,114,75,137,53,137,43,114,17,127,9,104,46,40});
   using(LinearGradientBrush fill=new LinearGradientBrush(new Point(20,20),new Point(105,130),Color.FromArgb(211,230,243),Color.FromArgb(46,70,105)))
    g.FillPolygon(fill,Points(tier==1?new float[]{64,8,79,50,108,104,83,91,76,126,52,126,45,91,20,104,49,50}:
     tier==2?new float[]{64,6,84,52,116,93,109,117,83,103,74,131,54,131,45,103,19,117,12,93,44,52}:
     new float[]{64,6,88,36,111,52,120,108,101,126,81,111,73,134,55,134,47,111,27,126,8,108,17,52,40,36}));
   Poly(g,Color.FromArgb(29,43,69),new float[]{64,17,78,57,74,116,54,116,50,57});
   Poly(g,light,new float[]{64,30,73,55,64,71,55,55});
   using(Pen p=new Pen(light,3)){g.DrawLines(p,Points(new float[]{23,106,42,82,48,107}));g.DrawLines(p,Points(new float[]{105,106,86,82,80,107}));}
   int[] cannons=tier==1?new[]{34,94}:tier==2?new[]{28,64,100}:new[]{20,42,64,86,108};
   foreach(int x in cannons){g.FillRectangle(Brushes.DarkSlateGray,x-4,47,8,43);using(Brush p=new SolidBrush(light))g.FillRectangle(p,x-3,44,6,10);g.FillRectangle(Brushes.LightSlateGray,x-2,60,4,21);}
   for(int i=0;i<tier+1;i++){int x=52+i*10-tier*5;using(Brush p=new SolidBrush(light))g.FillRectangle(p,x,119,6,12);}
   if(tier==3){using(Pen p=new Pen(Gold,2))g.DrawEllipse(p,53,78,22,22);g.FillEllipse(Brushes.LightYellow,60,85,8,8);}
  }return b;
 }
 static Bitmap EvolvedEnemy(int stage,int pattern){
  if(stage==0)return Ship(false,pattern);
  Bitmap b=new Bitmap(128,128,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
  using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;
   Color glow=stage==1?Pink:stage==2?Color.FromArgb(227,100,255):stage==3?Color.FromArgb(255,113,68):Color.FromArgb(255,65,100);
   Color steel=stage<3?Color.FromArgb(100,109,149):Color.FromArgb(114,90,113);
   float[] silhouette=stage==1?new float[]{64,17,86,34,111,8,104,81,89,118,76,91,64,111,52,91,39,118,24,81,17,8,42,34}:
    stage==2?new float[]{64,12,83,33,108,5,119,57,103,111,89,82,77,108,64,122,51,108,39,82,25,111,9,57,20,5,45,33}:
    stage==3?new float[]{64,11,83,27,111,4,100,44,124,55,111,100,96,79,91,119,75,100,64,125,53,100,37,119,32,79,17,100,4,55,28,44,17,4,45,27}:
    new float[]{64,3,78,23,105,5,101,31,124,23,111,64,125,91,106,86,97,124,79,106,64,127,49,106,31,124,22,86,3,91,17,64,4,23,27,31,23,5,50,23};
   Poly(g,Color.FromArgb(18,19,37),silhouette);
   using(LinearGradientBrush fill=new LinearGradientBrush(new Point(15,15),new Point(105,120),steel,Color.FromArgb(28,30,50)))g.FillPolygon(fill,Points(silhouette));
   using(Pen rim=new Pen(Color.FromArgb(160,steel),2))g.DrawPolygon(rim,Points(silhouette));
   Poly(g,Color.FromArgb(21,24,43),new float[]{64,23,88,53,83,87,64,108,45,87,40,53});
   Poly(g,glow,new float[]{43,53,59,59,55,66,42,61});Poly(g,glow,new float[]{85,53,69,59,73,66,86,61});
   using(Pen p=new Pen(glow,2)){g.DrawLines(p,Points(new float[]{28,45,34,76,41,94}));g.DrawLines(p,Points(new float[]{100,45,94,76,87,94}));}
   g.FillEllipse(Brushes.Black,55,74,18,18);using(Pen p=new Pen(glow,3))g.DrawEllipse(p,55,74,18,18);
   g.FillEllipse(Brushes.White,61,80,6,6);
   if(stage>=2){Poly(g,Color.LightSteelBlue,new float[]{47,74,54,83,49,99});Poly(g,Color.LightSteelBlue,new float[]{81,74,74,83,79,99});}
   for(int i=0;i<=pattern;i++){using(Brush p=new SolidBrush(glow))g.FillRectangle(p,48+i*9,29,5,7);}
   if(stage==4){using(Pen p=new Pen(Gold,2))g.DrawArc(p,40,35,48,58,10,160);}
  }return b;
 }
 public void DrawUpgrade(Graphics g,Entity e,float time){float x=e.P.X,y=e.P.Y;float pulse=30+(float)Math.Sin(time*5)*3;
  using(Pen p=new Pen(Color.FromArgb(80,Gold),3))g.DrawEllipse(p,x-pulse,y-pulse,pulse*2,pulse*2);
  using(Brush b=new SolidBrush(Color.FromArgb(225,42,32,20)))using(Pen p=new Pen(Gold,2)){
   PointF[] points=new PointF[6];for(int i=0;i<6;i++){double a=i*Math.PI/3;points[i]=new PointF(x+(float)Math.Cos(a)*25,y+(float)Math.Sin(a)*25);}g.FillPolygon(b,points);g.DrawPolygon(p,points);
  }Text(g,"U"+e.Type,x,y-14,21,Gold,true);Text(g,"CHOOSE PATH / "+e.Type,x,y+35,15,Gold,true);
 }
 public void SaveEvolutionPreview(string path){using(Bitmap b=new Bitmap(1200,700))using(Graphics g=Graphics.FromImage(b)){
  g.Clear(Color.FromArgb(7,13,29));g.SmoothingMode=SmoothingMode.AntiAlias;Text(g,"COSMOTWEEST / EVOLUTION",600,22,30,Color.White,true);
  for(int stage=0;stage<5;stage++){float x=120+stage*240;Sprite(g,Sprites["enemy"+stage+"_0"],new V(x,185),115,135,0);Text(g,"SECTOR 0"+(stage+1),x,276,16,Cyan,true);Text(g,World.EnemyNames[stage],x,304,19,Pink,true);}
  Text(g,"YOUR FLEET / OPTIONAL GOLD UPGRADE CORES",600,366,21,Gold,true);
  for(int tier=0;tier<4;tier++){float x=150+tier*300;Sprite(g,Sprites["player"+tier],new V(x,510),100+tier*9,118+tier*6,0);Text(g,World.HullNames[tier],x,598,21,Color.White,true);Text(g,tier==0?"STARTING ROCKET":World.UpgradeScores[tier-1].ToString("N0")+" POINTS",x,635,17,Gold,true);}
  b.Save(path);
 }}
}
}

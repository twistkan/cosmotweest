using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
namespace CosmoTweest {
public sealed partial class Art : IDisposable {
 public static readonly Color Cyan=Color.FromArgb(86,231,255),Gold=Color.FromArgb(255,196,89),Pink=Color.FromArgb(255,88,124),Green=Color.FromArgb(110,250,177);
 public readonly Bitmap Background=new Bitmap(960,780,System.Drawing.Imaging.PixelFormat.Format32bppPArgb); public readonly Dictionary<string,Bitmap> Sprites=new Dictionary<string,Bitmap>();
 readonly Dictionary<int,Font> fonts=new Dictionary<int,Font>(); public Font Font(int size){if(!fonts.ContainsKey(size))fonts[size]=new Font("Segoe UI",size,FontStyle.Regular,GraphicsUnit.Pixel);return fonts[size];}
 public Art(){Random r=new Random(77);using(Bitmap nebula=new Bitmap(240,195)){for(int y=0;y<195;y++)for(int x=0;x<240;x++){double cloud=(Math.Sin(x*.043+Math.Sin(y*.037)*3)+Math.Sin(y*.051+x*.015)+2)/4;double band=Math.Exp(-Math.Pow((x-120-Math.Sin(y*.018)*70)/85,2));double n=cloud*cloud*band;nebula.SetPixel(x,y,Color.FromArgb(5+(int)(n*17),9+(int)(n*12),24+(int)(n*35)));}using(Graphics g=Graphics.FromImage(Background)){g.InterpolationMode=InterpolationMode.Bilinear;g.DrawImage(nebula,0,0,960,780);for(int i=0;i<230;i++){using(Brush b=new SolidBrush(Color.FromArgb(r.Next(30,145),155,190,230)))g.FillEllipse(b,r.Next(960),r.Next(780),1,1);}}}
  for(int tier=0;tier<4;tier++)Sprites["player"+tier]=EvolvedPlayer(tier);Sprites["player"]=Ship(true,0);for(int stage=0;stage<5;stage++)for(int pattern=0;pattern<4;pattern++)Sprites["enemy"+stage+"_"+pattern]=EvolvedEnemy(stage,pattern);Sprites["boss"]=Boss();for(int i=0;i<5;i++)Sprites["rock"+i]=Rock(i);InitFleetArt();
 }
 static PointF[] Points(float[] a){PointF[] p=new PointF[a.Length/2];for(int i=0;i<p.Length;i++)p[i]=new PointF(a[i*2],a[i*2+1]);return p;}
 static void Poly(Graphics g,Color c,float[] p){using(Brush b=new SolidBrush(c))g.FillPolygon(b,Points(p));}
 static Bitmap Ship(bool player,int type){Bitmap b=new Bitmap(100,120,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;Color accent=player?Cyan:type==2?Gold:type==3?Color.MediumOrchid:Pink;
  Poly(g,Color.FromArgb(13,24,43),new float[]{50,5,66,44,94,94,71,88,61,108,39,108,29,88,6,94,34,44});
  using(LinearGradientBrush fill=new LinearGradientBrush(new Point(15,0),new Point(85,100),Color.FromArgb(192,210,227),Color.FromArgb(36,55,80)))g.FillPolygon(fill,Points(new float[]{50,7,62,42,86,88,64,77,59,99,41,99,36,77,14,88,38,42}));
  Poly(g,Color.FromArgb(43,62,88),new float[]{50,13,57,49,54,95,46,95,43,49});
  Poly(g,accent,new float[]{50,27,57,52,50,64,43,52});
  using(Pen p=new Pen(accent,2)){g.DrawLines(p,Points(new float[]{18,82,36,62,40,80}));g.DrawLines(p,Points(new float[]{82,82,64,62,60,80}));}
  using(Brush a=new SolidBrush(accent)){g.FillRectangle(a,32,86,5,9);g.FillRectangle(a,63,86,5,9);}
  using(Pen p=new Pen(Color.FromArgb(220,235,248),1))g.DrawLine(p,50,12,50,23);
  g.FillRectangle(Brushes.DimGray,40,98,7,9);g.FillRectangle(Brushes.DimGray,53,98,7,9);
 }if(!player)b.RotateFlip(RotateFlipType.Rotate180FlipNone);return b;}
 static Bitmap Boss(){Bitmap b=new Bitmap(240,190,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;Poly(g,Color.FromArgb(39,42,66),new float[]{120,14,181,43,228,126,190,157,149,138,120,180,91,138,50,157,12,126,59,43});Poly(g,Color.FromArgb(94,105,131),new float[]{120,20,170,52,204,126,179,132,144,112,120,164,96,112,61,132,36,126,70,52});Poly(g,Color.FromArgb(24,29,51),new float[]{120,31,141,87,120,150,99,87});using(Pen p=new Pen(Pink,4)){g.DrawEllipse(p,103,74,34,34);g.DrawLine(p,55,105,81,62);g.DrawLine(p,185,105,159,62);}g.FillEllipse(Brushes.White,115,86,10,10);for(int i=0;i<5;i++){g.FillRectangle(Brushes.Crimson,47+i*33,118,12,8);}}return b;}
 static Bitmap Rock(int seed){Bitmap b=new Bitmap(100,100,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);Random r=new Random(seed+91);using(Graphics g=Graphics.FromImage(b)){g.SmoothingMode=SmoothingMode.AntiAlias;PointF[] p=new PointF[12];for(int i=0;i<12;i++){double a=i*Math.PI/6;float rad=r.Next(32,46);p[i]=new PointF(50+(float)Math.Cos(a)*rad,50+(float)Math.Sin(a)*rad);}using(LinearGradientBrush br=new LinearGradientBrush(new Point(10,10),new Point(90,90),Color.FromArgb(151,132,119),Color.FromArgb(47,47,61)))g.FillPolygon(br,p);using(Pen pen=new Pen(Color.FromArgb(175,154,129),2))g.DrawPolygon(pen,p);for(int i=0;i<7;i++){int x=r.Next(25,65),y=r.Next(23,65),s=r.Next(8,19);g.FillEllipse(BrushesDark,x,y,s,s);using(Pen pen=new Pen(Color.FromArgb(133,119,108),1))g.DrawArc(pen,x,y,s,s,10,150);}}return b;}
 static readonly Brush BrushesDark=new SolidBrush(Color.FromArgb(68,66,70));
 public void Text(Graphics g,string s,float x,float y,int size,Color c,bool center=false){using(Brush b=new SolidBrush(c)){if(center)x-=g.MeasureString(s,Font(size)).Width/2;g.DrawString(s,Font(size),b,x,y);}}
 public void Sprite(Graphics g,Bitmap b,V p,float w,float h,float angle){GraphicsState st=g.Save();g.TranslateTransform(p.X,p.Y);g.RotateTransform(angle);g.DrawImage(b,-w/2,-h/2,w,h);g.Restore(st);}
 public void DrawWorld(Graphics g,World w,float time){
  foreach(Entity e in w.Entities){if(DrawFleetEntity(g,e,time))continue;if(e.Kind=="enemy")Sprite(g,Sprites["enemy"+e.Stage+"_"+e.Type],e.P,54+e.Stage*3,65+e.Stage*2,(float)Math.Sin(e.Age)*8);else if(e.Kind=="boss")Sprite(g,Sprites["boss"],e.P,216,171,0);else if(e.Kind=="rock")Sprite(g,Sprites["rock"+((int)(e.Seed*10)%5)],e.P,e.Radius*2.3f,e.Radius*2.3f,e.Age*23);else if(e.Kind=="upgrade"){DrawUpgrade(g,e,time);}else if(e.Kind=="crate"||e.Kind=="pickup"){
   Color c=e.Type==0?Gold:e.Type==1?Green:Color.MediumOrchid;float x=e.P.X,y=e.P.Y;
   using(Pen p=new Pen(c,2))using(Brush b=new SolidBrush(Color.FromArgb(30,c))){if(e.Kind=="crate"){g.FillRectangle(b,x-22,y-22,44,44);g.DrawRectangle(p,x-22,y-22,44,44);g.DrawLine(p,x-22,y-22,x-10,y-10);g.DrawLine(p,x+22,y+22,x+10,y+10);}else {g.FillEllipse(b,x-21,y-21,42,42);g.DrawEllipse(p,x-18,y-18,36,36);}}
   Text(g,e.Type==0?"III":e.Type==1?"+":"B",x,y-15,23,c,true);
  }else{Color c=e.Kind=="hostile"?Pink:e.Kind=="rescue"?Green:e.Type==4?Green:e.Type>=3?Gold:e.Type==2?Color.FromArgb(176,164,255):Cyan;using(Pen glow=new Pen(Color.FromArgb(60,c),10))using(Pen p=new Pen(c,3)){V tail=e.P-e.Vel.Unit*(e.Kind=="hostile"?7:17);g.DrawLine(glow,e.P.X,e.P.Y,tail.X,tail.Y);g.DrawLine(p,e.P.X,e.P.Y,tail.X,tail.Y);}}}
  foreach(Particle p in w.Particles){Color c=Palette(p.Color);using(Brush b=new SolidBrush(Color.FromArgb((int)(200*Math.Min(1,p.Life/p.Max)),c)))g.FillEllipse(b,p.P.X-p.Size/2,p.P.Y-p.Size/2,p.Size,p.Size);}
  if(!w.Lost&&(w.Invulnerable<=0||(int)(time*14)%2==0)){float a=(float)(Math.Atan2(w.Aim.X-w.Player.X,-(w.Aim.Y-w.Player.Y))*180/Math.PI);Sprite(g,Sprites[w.PlayerSprite],w.Player,52+w.Tier*10,64+w.Tier*6,a);if(w.Invulnerable>0)using(Pen p=new Pen(Color.FromArgb(90,Cyan),2))g.DrawEllipse(p,w.Player.X-35,w.Player.Y-35,70,70);}
  DrawFleetCompanions(g,w,time);
  if(w.BombRing>0)using(Pen p=new Pen(Color.FromArgb((int)(w.BombRing*220),Cyan),4)){float r=(.8f-w.BombRing)*650;g.DrawEllipse(p,w.Player.X-r,w.Player.Y-r,r*2,r*2);}
  if(w.Flash>0)using(Brush b=new SolidBrush(Color.FromArgb((int)(w.Flash*130),Pink)))g.FillRectangle(b,0,80,960,700);
 }
 public static Color Palette(int i){return i==0?Cyan:i==1?Pink:i==2?Gold:i==3?Gold:i==4?Green:Color.MediumOrchid;}
 public void Dispose(){Background.Dispose();foreach(Bitmap b in Sprites.Values)b.Dispose();foreach(Font f in fonts.Values)f.Dispose();}
}
}

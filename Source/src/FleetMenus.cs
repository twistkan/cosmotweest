using System;
using System.Drawing;
using System.Windows.Forms;
namespace CosmoTweest {
public sealed partial class Game {
 void AcceptPath(int branch){if(world.ChooseUpgrade(branch)){page="game";firing=boost=bomb=mouseSteering=false;keys.Clear();selected=0;}}
 bool DrawFleetMenus(Graphics g){
  if(page=="upgrade"){
   Title(g,"RANK "+world.PendingTier+" UPGRADE / ACTION PAUSED","CHOOSE YOUR HULL");
   string[][] notes={new[]{"20% faster flight","Rapid cannons","2.2s boost cooldown"},new[]{"Homing missiles","Explosive splash damage","10% slower flight"},new[]{"Regenerating shield","Absorbs incoming damage","20% slower flight"}};
   for(int branch=0;branch<3;branch++){int path=branch;float x=45+branch*300;RectangleF r=new RectangleF(x,265,270,370);bool hover=selected==branch||r.Contains(mouse.X,mouse.Y);Color c=Art.PathColor(branch);
    using(Brush b=new SolidBrush(Color.FromArgb(hover?37:17,hover?52:30,hover?68:49)))g.FillRectangle(b,r);
    using(Pen p=new Pen(hover?c:Color.SlateGray,hover?3:1))g.DrawRectangle(p,r.X,r.Y,r.Width,r.Height);
    art.Sprite(g,art.Sprites["path"+branch+"_"+world.PendingTier],new V(x+135,369),112,112,0);
    art.Text(g,World.Paths[branch],x+135,450,20,c,true);for(int n=0;n<3;n++)art.Text(g,notes[branch][n],x+135,495+n*33,17,Color.LightSteelBlue,true);
    buttons.Add(new ButtonArea{R=r,Label=World.Paths[branch],Action=delegate{AcceptPath(path);}});
   }
   art.Text(g,"Click a hull / LEFT-RIGHT + ENTER / keys 1-3",480,677,19,Art.Cyan,true);
   art.Text(g,"You may choose a different path at the next gold core.",480,712,17,Color.LightSteelBlue,true);return true;
  }
  if(page=="flightsettings"){
   Title(g,"CHOOSE YOUR CHALLENGE","FLIGHT SETUP");
   Button(g,"DIFFICULTY / "+World.Difficulties[settings.Difficulty],265,delegate{settings.Difficulty=(settings.Difficulty+1)%3;Save();});
   Button(g,settings.MouseFlight?"CONTROLS / MOUSE FLIGHT":"CONTROLS / KEYBOARD + AIM",325,delegate{settings.MouseFlight=!settings.MouseFlight;mouseSteering=false;Save();});
   Button(g,"AUTO-FIRE / "+(settings.AutoFire?"ON":"OFF"),385,delegate{settings.AutoFire=!settings.AutoFire;Save();});
   Button(g,"BACK",465,delegate{page="settings";});
   string description=settings.Difficulty==0?"Slower enemies, lighter damage, more starting supplies.":settings.Difficulty==2?"Faster enemies, harder hits, fewer starting supplies.":"The standard CosmoTweest challenge.";
   art.Text(g,description,480,556,20,Art.Gold,true);
   art.Text(g,"Difficulty applies to your next campaign or sector selection.",480,601,18,Color.LightSteelBlue,true);
   art.Text(g,"High scores are kept separately for each difficulty.",480,635,18,Color.LightSteelBlue,true);
   art.Text(g,"Auto-fire uses your ammunition. Switch it off to conserve rounds.",480,680,17,Art.Cyan,true);return true;
  }return false;
 }
 void FleetHud(Graphics g){
  art.Text(g,World.Difficulties[world.Difficulty],812,112,13,Art.Gold);
  art.Text(g,"DRONES "+world.DroneCount+"/2",812,132,13,Art.Cyan);
  if(world.MaxShield>0){Bar(g,24,136,140,world.Shield/world.MaxShield,Art.Green);art.Text(g,"SHIELD "+(int)world.Shield,174,129,12,Art.Green);}
  Entity mini=world.Entities.Find(delegate(Entity e){return !e.Dead&&e.Kind=="miniboss";});
  if(mini!=null){art.Text(g,World.MiniNames[mini.Type]+(world.Armored(mini)?" / ARMORED":" / EXPOSED"),480,93,16,Art.Pink,true);Bar(g,250,121,460,mini.HP/mini.MaxHP,Art.Pink);}
 }
}
}

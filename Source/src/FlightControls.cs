using System;
namespace CosmoTweest {
static class FlightControls {
 public const float MouseSpeed=430;
 // Return a fractional movement vector for the final step, so following the
 // cursor never overshoots or oscillates, even during an evasive boost.
 public static V Follow(V player,V pointer,float dt,float speed){
  V target=new V(Math.Max(24,Math.Min(World.W-24,pointer.X)),Math.Max(105,Math.Min(World.H-28,pointer.Y)));
  V delta=target-player;
  if(dt<=0||delta.Length<.1f)return new V();
  return delta.Unit*Math.Min(1,delta.Length/(speed*dt));
 }
}
}

using System;
using System.IO;
using System.Runtime.InteropServices;
namespace CosmoTweest {
// A small original synthesizer. Separate music/SFX gains, no codecs or asset files.
public sealed class Audio : IDisposable {
 [StructLayout(LayoutKind.Sequential)] struct Format {public ushort tag,channels;public uint rate,bytes;public ushort align,bits,extra;}
 [StructLayout(LayoutKind.Sequential)] struct Header {public IntPtr data;public uint length,recorded;public IntPtr user;public uint flags,loops;public IntPtr next,reserved;}
 [DllImport("winmm.dll")]static extern int waveOutOpen(out IntPtr h,uint id,ref Format f,IntPtr cb,IntPtr instance,uint flags);
 [DllImport("winmm.dll")]static extern int waveOutPrepareHeader(IntPtr h,IntPtr p,uint s);
 [DllImport("winmm.dll")]static extern int waveOutWrite(IntPtr h,IntPtr p,uint s);
 [DllImport("winmm.dll")]static extern int waveOutUnprepareHeader(IntPtr h,IntPtr p,uint s);
 [DllImport("winmm.dll")]static extern int waveOutReset(IntPtr h);
 [DllImport("winmm.dll")]static extern int waveOutClose(IntPtr h);
 IntPtr device;IntPtr[] data=new IntPtr[4],headers=new IntPtr[4];int size=Marshal.SizeOf(typeof(Header));long sample;Random random=new Random(18);readonly object gate=new object();float tone,remaining,noise;public float Music=0,Effects=0;public bool Available;
 public Audio(){try{Format f=new Format{tag=1,channels=1,rate=22050,bytes=44100,align=2,bits=16};Available=waveOutOpen(out device,UInt32.MaxValue,ref f,IntPtr.Zero,IntPtr.Zero,0)==0;if(!Available)return;for(int i=0;i<4;i++){data[i]=Marshal.AllocHGlobal(512*2);headers[i]=Marshal.AllocHGlobal(size);Marshal.StructureToPtr(new Header{data=data[i],length=1024},headers[i],false);waveOutPrepareHeader(device,headers[i],(uint)size);}Pump();}catch{Available=false;}}
 public void SetVolumes(float music,float effects){lock(gate){bool silence=music==0&&effects==0&&(Music>0||Effects>0);Music=music;Effects=effects;if(silence&&Available){waveOutReset(device);remaining=0;}}}
 public void Play(string name){lock(gate){tone=name=="shot"?850:name=="upgrade"?1450:name=="pickup"?1100:name=="boost"?220:75;remaining=name=="shot"?.055f:name=="pickup"?.2f:.3f;noise=name=="explode"||name=="hurt"||name=="bomb"?.8f:.1f;}}
 public void Pump(){if(!Available)return;lock(gate){for(int i=0;i<4;i++){Header h=(Header)Marshal.PtrToStructure(headers[i],typeof(Header));if((h.flags&16)!=0&&(h.flags&1)==0)continue;short[] buffer=new short[512];int[] notes={45,52,57,52,41,48,53,48,43,50,55,50,40,47,52,59};for(int j=0;j<buffer.Length;j++,sample++){double t=sample/22050.0;int beat=(int)(t*3);double hz=440*Math.Pow(2,(notes[(beat/2)%notes.Length]-69)/12.0);double env=Math.Exp(-(t*3-beat)*5);double music=(Math.Sin(t*hz*Math.PI*2)*.12+Math.Sin(t*hz*4*Math.PI)*.035)*(.4+.6*env);double kick=Math.Sin(t*2*Math.PI*(48+80*env))*env*.1;double sfx=0;if(remaining>0){sfx=(Math.Sin(t*tone*2*Math.PI)*(1-noise)+(random.NextDouble()*2-1)*noise)*Math.Min(1,remaining*18)*.3;remaining-=1f/22050;tone*=.9997f;}buffer[j]=(short)(Math.Max(-1,Math.Min(1,(music+kick)*Music+sfx*Effects))*32760);}Marshal.Copy(buffer,0,data[i],buffer.Length);waveOutWrite(device,headers[i],(uint)size);}}}
 public void Dispose(){if(device==IntPtr.Zero)return;waveOutReset(device);for(int i=0;i<4;i++)if(headers[i]!=IntPtr.Zero){waveOutUnprepareHeader(device,headers[i],(uint)size);Marshal.FreeHGlobal(headers[i]);Marshal.FreeHGlobal(data[i]);}waveOutClose(device);device=IntPtr.Zero;}
}
}

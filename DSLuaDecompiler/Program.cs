﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using luadec;
using luadec.Utilities;

namespace luadec
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Encoding outEncoding = Encoding.UTF8;
                    // Super bad arg parser until I decide to use a better libary
                    bool writeFile = true;
                    string outfilename = null;
                    string infilename = null;
                    int arg = 0;
                    infilename = args[i];
                    Console.WriteLine($"Processing File {infilename} : Number = {i + 1}");
                    outfilename = Path.GetFileNameWithoutExtension(infilename) + ".dec.lua";

                    Console.OutputEncoding = outEncoding;
                    //infilename = $@"E:\SteamLibrary\steamapps\common\DARK SOULS III\Game\script\aicommon-luabnd-dcx\script\ai\out\bin\goal_list.lua";
                    //infilename = $@"C:\Users\katalash\Downloads\script_interroot (1)\script_interroot\ai\out\approach_target.lua.out";
                    //infilename = $@"E:\soulsmodsstuff\soulsmodsgh\og\DSMapStudio\DecompileAllScripts\bin\Debug\net5.0\output\mismatches\aicommon.luabnd\walk_around_on_failed_path.lua";
                    using (FileStream stream = File.OpenRead(infilename))
                    {
                        BinaryReaderEx br = new BinaryReaderEx(false, stream);
                        var lua = new LuaFile(br);
                        IR.Function main = new IR.Function();
                        //LuaDisassembler.DisassembleFunction(lua.MainFunction);
                        if (lua.Version == LuaFile.LuaVersion.Lua50)
                        {
                            LuaDisassembler.GenerateIR50(main, lua.MainFunction);
                            outEncoding = Encoding.GetEncoding("shift_jis");
                        }
                        else if (lua.Version == LuaFile.LuaVersion.Lua51HKS)
                        {
                            LuaDisassembler.GenerateIRHKS(main, lua.MainFunction);
                            outEncoding = Encoding.UTF8;
                        }
                        else if (lua.Version == LuaFile.LuaVersion.Lua53Smash)
                        {
                            LuaDisassembler.GenerateIR53(main, lua.MainFunction, true);
                            outEncoding = Encoding.UTF8;
                        }

                        if (writeFile)
                        {
                            File.WriteAllText(outfilename, main.ToString(), outEncoding);
                        }
                        else
                        {
                            Console.WriteLine(main.ToString());
                        }
                    }

                    Console.WriteLine($"Finished writing to : {outfilename}");
                }
                catch (Exception exp)
                {

                    Console.WriteLine(exp);
                }
            }
            Console.ReadLine();
        }
    }
}

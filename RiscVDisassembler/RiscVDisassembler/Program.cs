using System;
using System.Collections.Generic;
using System.IO;

namespace RiscVDisassembler
{
    internal class Program
    {
        private static void Main()
        {
            List<uint> program = HexFileDecoder.DecodeHexFile();

            if (program.Count > 0) {
                DisassembleProgram(program);
            } 
        }

        private static void DisassembleProgram(List<uint> program) {
            uint pc = 0;

            Console.WriteLine("pc \t\t raw instruction \t disassembled instruction");
            Console.WriteLine("-------------------------------------------------------------------");

            for (int i = 0; i < program.Count; i += 1) {
                uint instruction = program[i];
                string disassembledInstruction = InstructionDecoder.Disassemble(instruction);
                // Display: PC | raw instruction | disassembled instruction
                Console.WriteLine($"0x{pc:x8}: \t 0x{instruction:x8} \t\t {disassembledInstruction}");

                pc += 4;
            }
        }
    }
}
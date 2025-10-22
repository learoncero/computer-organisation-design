using System;
using System.Collections.Generic;
using System.IO;

namespace RiscVDisassembler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "Resources\\program.txt";
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' not found.");
                return;
            }

            List<uint> program = LoadProgram(filePath);
            DisassembleProgram(program);
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

        private static List<uint> LoadProgram(string filePath)
        {
            List<uint> program = new List<uint>();

            try
            {
                foreach (string line in File.ReadAllLines(filePath))
                {
                    // skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // split by commas
                    string[] parts = line.Split(',');

                    foreach (string part in parts)
                    {
                        string trimmed = part.Trim();

                        if (string.IsNullOrEmpty(trimmed))
                            continue;

                        // parse hex to uint
                        uint instruction = Convert.ToUInt32(trimmed, 16);
                        program.Add(instruction);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading program: {ex.Message}");
            }

            return program;
        }
    }
}
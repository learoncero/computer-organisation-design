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

            foreach (uint instruction in program)
            {
                string disassembledInstruction = InstructionDecoder.Disassemble(instruction);
                Console.WriteLine($"0x{instruction:x8} : {disassembledInstruction}");
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
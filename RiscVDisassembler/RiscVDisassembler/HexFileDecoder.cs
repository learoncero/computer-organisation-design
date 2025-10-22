using System;
using System.Collections.Generic;
using System.Text;

namespace RiscVDisassembler {
    internal class HexFileDecoder {
        public const string filePath = "Resources\\program.hex";
        public const int instructionLength = 8;

        public static List<uint> DecodeHexFile() {
            List<uint> program = [];
            
            if (!File.Exists(filePath)) {
                Console.WriteLine($"Error: File '{filePath}' not found.");
                return program;
            }

            try {
                foreach (string line in File.ReadAllLines(filePath)) {
                    if (line.StartsWith(":00000001")) {
                        continue;
                    }

                    // remove first 9 characters (: + byte count + address) and last 2 (checksum)
                    string cleanLine = line[9..^2];

                    // skip empty lines
                    if (string.IsNullOrWhiteSpace(cleanLine)) {
                        continue;
                    }

                    for (int i = 0; i < cleanLine.Length; i += instructionLength) {
                        string instruction = cleanLine.Substring(i, instructionLength);
                        string swappedInstruction = instruction[6..8] + instruction[4..6] + instruction[2..4] + instruction[0..2];

                        uint instructionValue = Convert.ToUInt32(swappedInstruction, 16);
                        program.Add(instructionValue);

                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading program: {ex.Message}");
            }

            return program;
        }
    }
}

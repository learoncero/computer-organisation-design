#include <iostream>
#include "InsMem.hpp"

int main() {
    std::cout << "Hello and welcome to the RISC-V Disassembler!\n" << std::endl;

    InsMem mem;
    std::cout << mem.fetchInstruction(0x00000000) << std::endl;

    return 0;
}
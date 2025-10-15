#include <iostream>
#include "InsMem.hpp"

InsMem::InsMem() : x(0) {
    std::cout << "InsMem constructor called" << std::endl;
    uint32_t* pMem = (uint32_t*)_memory;
    pMem[0] = 0xA0001223;
    pMem[1] = 0xCF040301;
}

uint32_t InsMem::fetchInstruction(uint32_t addr) {
    return *((uint32_t*) &_memory[addr]);

    // entweder man hat ein add oder addi und man soll erkennen, ob es ein add oder addi ist
    // bei add drei register ausgeben und bei addi zwei register und eine immediate
}
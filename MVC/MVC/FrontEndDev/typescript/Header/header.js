"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.testHeader = void 0;
/* Content here */
function testHeader() {
    var testing = { message: "Hello World2", test2: 12 };
    return testing;
}
exports.testHeader = testHeader;
document.addEventListener("ready", function () {
    var test = testHeader();
});
//# sourceMappingURL=header.js.map
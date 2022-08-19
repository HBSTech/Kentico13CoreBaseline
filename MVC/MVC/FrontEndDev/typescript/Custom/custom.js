"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.testCustom = void 0;
/* Content here */
function testCustom() {
    var testing = { message: "Hello World" };
    return testing;
}
exports.testCustom = testCustom;
document.addEventListener("ready", function () {
    var test = testCustom();
});
//# sourceMappingURL=custom.js.map
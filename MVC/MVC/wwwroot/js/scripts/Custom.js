/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it uses a non-standard name for the exports (exports).
(() => {
var exports = __webpack_exports__;
var __webpack_unused_export__;

__webpack_unused_export__ = ({ value: true });
__webpack_unused_export__ = void 0;
/* Content here */
function testCustom() {
    var testing = { message: "Hello World" };
    return testing;
}
__webpack_unused_export__ = testCustom;
document.addEventListener("ready", function () {
    var test = testCustom();
});

})();

/******/ })()
;
//# sourceMappingURL=Custom.js.map
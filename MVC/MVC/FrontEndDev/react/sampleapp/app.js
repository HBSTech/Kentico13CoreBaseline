"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ReactMethods = void 0;
var ReactDOM = require("react-dom");
var React = require("react");
var HelloWorld_1 = require("./components/HelloWorld");
var ReactMethods = /** @class */ (function () {
    function ReactMethods() {
    }
    ReactMethods.SampleApp = function (containerID) {
        ReactDOM.render(React.createElement(HelloWorld_1.HelloWorld, { CustomMessage: "Testing etc" }), document.getElementById(containerID));
    };
    return ReactMethods;
}());
exports.ReactMethods = ReactMethods;
// Can access your methods with window.ReactMethods.SampleApp("MyContainer")
window.ReactMethods = ReactMethods;
//# sourceMappingURL=app.js.map
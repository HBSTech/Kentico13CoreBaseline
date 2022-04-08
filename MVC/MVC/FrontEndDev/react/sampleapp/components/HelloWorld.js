"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
exports.HelloWorld = void 0;
var React = require("react");
var HelloWorld = /** @class */ (function (_super) {
    __extends(HelloWorld, _super);
    function HelloWorld(props) {
        var _this = _super.call(this, props) || this;
        _this.DisplayMessageClick = function () {
            _this.setState({
                DisplayedMessage: true
            });
        };
        _this.state = {
            DisplayedMessage: false
        };
        return _this;
    }
    HelloWorld.prototype.componentDidMount = function () {
    };
    HelloWorld.prototype.componentDidUpdate = function () {
    };
    HelloWorld.prototype.render = function () {
        return React.createElement("div", null,
            "Hello world!",
            this.state.DisplayedMessage &&
                React.createElement("p", null, this.props.CustomMessage),
            !this.state.DisplayedMessage &&
                React.createElement("button", { type: "button", onClick: this.DisplayMessageClick }, "Show Message"));
    };
    return HelloWorld;
}(React.Component));
exports.HelloWorld = HelloWorld;
//# sourceMappingURL=HelloWorld.js.map
import * as React from "react";
import ReactDOM = require("react-dom");

export class HelloWorld extends React.Component<IHelloWorldProps, IHelloWorldState> {
    constructor(props) {
        super(props);

        this.state = {
            DisplayedMessage: false
        }
    }
    componentDidMount() {
    }
    componentDidUpdate() {
    }
    render() {
        return <div>
            Hello world!
            {this.state.DisplayedMessage && 
                <p>{this.props.CustomMessage}</p>
            }
            {!this.state.DisplayedMessage &&
                <button type="button" onClick={this.DisplayMessageClick}>Show Message</button>
            }
            </div>
    }

    DisplayMessageClick = () => {
        this.setState({
            DisplayedMessage: true
        });
    }
}
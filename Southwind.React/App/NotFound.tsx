﻿import * as React from 'react'
import * as Navigator from "../../Framework/Signum.React/Scripts/Navigator"

export default class NotFound extends React.Component {

    componentWillMount() {
        if (Navigator.currentUser == null) {
            debugger;
            Navigator.history.replace("~/auth/login", { back: Navigator.history.location });
        }
    }

    render() {
        return (
            <div>
                <h3>404 <small>Not Found</small></h3>
            </div>
        );
    }
}

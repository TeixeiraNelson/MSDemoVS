import React from "react";
import ReactDOM from "react-dom";

import ChartTable from "statistics/ChartTable";

const TestComponent = () => <h1>THIS IS A MICRO FRONT END</h1>;

ReactDOM.render(
    <TestComponent />,
    document.getElementById("component-container")
);
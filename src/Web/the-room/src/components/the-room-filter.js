import React from 'react';

const TheRoomFilter = (props) => {
    return <React.Fragment>
                <form className="row g-3 mb-3">
                    <div className="col-auto">
                        <label htmlFor={props.FilterId}>{props.PlaceHolder}</label>
                        <div className="input-group mb-3">
                            <input type="text" className="form-control" value={props.Value} onChange={props.HandleFilter} id={props.FilterId} placeholder={props.PlaceHolder} />
                            <span className="input-group-append px-2">
                                <button type="button" onClick={props.HandleReset} className="btn btn-outline-dark">{props.ButtonLabel}</button>
                            </span>
                        </div>
                    </div>
                </form>
            </React.Fragment>;
};

TheRoomFilter.defaultProps = {
    Valeu: "",
    FilterId: "filter",
    PlaceHolder: "Filter",
    ButtonLabel: "Reset",
    HandleFilter: null,
    HandleReset: null
};
export default TheRoomFilter;
import React from 'react';

const Forbidden = (props) => {
    return <React.Fragment>
                <div className="col-md-6">
                    <div className="d-flex">
                        <div className="d-flex justify-content-center">
                            <div class="card text-center">
                                <div class="card-body">
                                    <h2>You are Forbidden</h2>
                                    <button className="btn btn-primary btn-block">Get Access Now</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>;
};

Forbidden.defaultProps = {
};
export default Forbidden;
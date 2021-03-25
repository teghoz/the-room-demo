import React from 'react';

const TheRoomService = (props) => {
    return <React.Fragment>
                <div className="services-description">
                    <h2>{props.Title}</h2>
                    <span>{props.Description}</span>
                </div>            
            </React.Fragment>;
};

TheRoomService.defaultProps = {
    Title: "Siteconstructor.io",
    Description: "Description"
};
export default TheRoomService;
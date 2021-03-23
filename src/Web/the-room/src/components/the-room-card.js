import React from 'react';
import TheRoomPromoCode from './the-room-promo-code';
import TheRoomService from './the-room-service';

const TheRoomCard = (props) => {
    return <div className="card mb-4">
                <div className="card-body d-flex justify-content-between">
                    <div className="p-2 align-items-start">
                        <TheRoomService Title={props.Title} Description={props.Description}></TheRoomService>
                    </div>
                    <div className="p-2 align-items-end">
                        <TheRoomPromoCode 
                            Id={props.Id}
                            HandlePromoCode={props.HandlePromoCode} 
                            HandleCopy={props.HandleCopy}
                            PromoCode={props.Promocode} />
                    </div>
                    <div className="p-2 align-items-end">
                        <button onClick={props.HandleBonus} className="btn btn-primary btn-block">Activate Bonus</button>
                    </div>
                </div>
            </div>;
};

TheRoomCard.defaultProps = {
    Id: 0,
    Title: "Siteconstructor.io",
    Description: "Description",
    PromoCode: '',
    HandleCopy: null,
    HandleBonus: null,
    HandlePromoCode: null,
    
};
export default TheRoomCard;
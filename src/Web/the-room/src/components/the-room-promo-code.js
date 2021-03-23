import React from 'react';

const TheRoomPromoCode = (props) => {
    return <React.Fragment>
                <label htmlFor="promo"><small>{props.LabelTitle}</small></label>
                <div className="input-group mb-3">
                    <input type="text" id={'promo'+ props.Id} data-id={props.Id} onChange={props.HandlePromoCode} className="form-control promo border-right-0" placeholder={props.PlaceHolder} defaultValue={props.PromoCode}/>
                    <span id={'promo-btn'+ props.Id} className="input-group-append bg-white border-left-0" data-id={props.Id} onClick={props.HandleCopy}>
                        <span className="input-group-text bg-transparent" data-id={props.Id}>
                            <i className={props.Icon} data-id={props.Id}></i>
                        </span>
                    </span>
                </div>
            </React.Fragment>;
};

TheRoomPromoCode.defaultProps = {
    Id: 0,
    LabelTitle: "PROMOCODE",
    PlaceHolder: "promocodes",
    PromoCode: "itpromocodes",
    Icon: "icon ion-ios-copy",
    Title: "Siteconstructor.io",
    Description: "Description",
    HandlePromoCode: null,
    HandleCopy: null
};

export default TheRoomPromoCode;
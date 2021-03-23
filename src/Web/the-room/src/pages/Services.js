import AuthGuard from '../shared/services/auth.guard'
import React from 'react';
import ServiceListService from '../shared/services/servicelist-service'
import TheRoomCard from '../components/the-room-card'
import TheRoomFilter from '../components/the-room-filter'

export default class Services extends React.Component{
    constructor(props){
        super(props);
        this.state = {
            headerOption: {},
            authenticatedUser: {},
            items: [],
            serviceListItems: [],
            filterValue: ''
        }
    }
    componentDidMount(){
        let authGuard = new AuthGuard(process.env.REACT_APP_IDENTITY_URL, window.location, this.props.history);
        authGuard.authorizedCallback();
        if (authGuard.IsAuthorized === false || authGuard.IsAuthorized === 'false' || authGuard.IsAuthorized === null) {
            this.props.history.push('UnAuthorized');
        }
        
        let headers = authGuard.setHeaders();
        this.setState({headerOption: headers});
        this.setState({authenticatedUser: authGuard.UserData});

        let serviceListService = new ServiceListService(process.env.REACT_APP_SERVICE_LIST_SERVICE, headers);
        serviceListService.getServiceListItems().then(data => {
            this.setState({serviceListItems: data.data.data});
            this.setState({items: data.data.data});
        });
    }

    handleCopy = (event) => {
        console.log("state: ", this.state);
        navigator.clipboard.writeText(this.state.serviceListItems.find((item) => item.id == event.target.dataset.id).inputedPromo) 
    }

    handleFiltering = (event) => {
        this.setState({ filterValue: event.target.value });
        this.setState({items: this.handleFilterLogic()});
        if(this.state.filterValue === ""){
            this.setState({items: this.state.serviceListItems});
        }
    }

    handleFilterLogic = () => {
        return this.state.serviceListItems.filter(filter => 
            filter.name.toLowerCase().indexOf(this.state.filterValue.toLowerCase()) > -1 ||
            filter.description.toLowerCase().indexOf(this.state.filterValue.toLowerCase()) > -1);
    }

    handleReset = () => {
        this.setState({filterValue: ''});
        this.setState({items: this.state.serviceListItems});
    }

    handleActiveBonus(){

    }

    handlePromoCode = (event) => {
        let list = [...this.state.serviceListItems]
        list.find((item) => item.id == event.target.dataset.id).inputedPromo = event.target.value;
        this.setState({serviceListItems: list});
    }

    render(){
        return (<React.Fragment>
                <div className="">
                    <div className="d-flex justify-content-end mt-2 mr-2">
                        <h6>Welcome {this.state.authenticatedUser.firstname}</h6> 
                    </div>
                    <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                        <h1 className="">Services</h1>
                    </div>
                    <TheRoomFilter 
                        Value={this.state.filterValue} 
                        HandleFilter={this.handleFiltering} 
                        HandleReset={this.handleReset}
                        HandleBonus={this.handleActiveBonus}/>
                    <div className="row">
                        <div className="col">
                            <div className="services">
                                {
                                    this.state.items.map(list => {
                                        return <TheRoomCard 
                                        key={list.id}
                                        Id={list.id}
                                        Title={list.name}
                                        Description={list.description}
                                        HandlePromoCode={this.handlePromoCode}
                                        PromoCode={list.id}
                                        HandleCopy={this.handleCopy} />
                                    })
                                }
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>);
    }
}
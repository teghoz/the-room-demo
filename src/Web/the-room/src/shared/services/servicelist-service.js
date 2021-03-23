import axios from 'axios';

export default class ServiceListService
{
    constructor(baseUrl, headers){
        this.baseUrl = baseUrl;
        this.options = headers;
    }
    async getServiceListItems(){
        return await axios.get(`${this.baseUrl}/api/v1/ServiceList/items`, { headers: this.options})
    }
}
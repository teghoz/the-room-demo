import axios from 'axios';

export default class AuthGuard{
    actionUrl = "";
    storage = localStorage;
    authorityUrl = '';
    UserData = {};
    IsAuthorized = false;
    headers = {};
    location = null;
    history = null;

    constructor(_identityUrl, _location, _history) {
        this.location = _location;
        this.headers = {
            'Content-Type': 'application/json',
            'Accept': 'text/plain'
        };

        this.authorityUrl = _identityUrl;
        localStorage.setItem('IdentityUrl', this.authorityUrl);
        this.history = _history;

        if (localStorage.getItem('IsAuthorized') !== '') {
            this.IsAuthorized = localStorage.getItem('IsAuthorized');
            this.UserData = JSON.parse(localStorage.getItem('userData'));
        }
    }    

    getToken() {
        return localStorage.getItem('authorizationData');
    }

    resetAuthorizationData() {
        localStorage.setItem('authorizationData', '');
        localStorage.setItem('authorizationDataIdToken', '');

        this.IsAuthorized = false;
        localStorage.setItem('IsAuthorized', false);
    }

    setAuthorizationData(token, id_token) {
        if (localStorage.getItem('authorizationData') !== '') {
            localStorage.setItem('authorizationData', '');
        }

        localStorage.setItem('authorizationData', token);
        localStorage.setItem('authorizationDataIdToken', id_token);
        this.IsAuthorized = true;
        localStorage.setItem('IsAuthorized', true);

        this.getUserData()
            .then(data => {
                this.UserData = data.data;
                console.log('user data: ', this.UserData)
                localStorage.setItem('userData', JSON.stringify(data.data));
                window.location.href = this.location.origin;
            })
            .catch(error => this.handleError(error));
    }

    authorize() {
        this.resetAuthorizationData();

        let authorizationUrl = this.authorityUrl + '/connect/authorize';
        let client_id = 'the-room-react';
        let redirect_uri = this.location.origin + '/';
        let response_type = 'id_token token';
        let scope = 'openid profile servicelist';
        let nonce = 'N' + Math.random() + '' + Date.now();
        let state = Date.now() + '' + Math.random();

        localStorage.setItem('authStateControl', state);
        localStorage.setItem('authNonce', nonce);

        console.log("redirecturi: ", redirect_uri);

        let url =
            authorizationUrl + '?' +
            'response_type=' + encodeURI(response_type) + '&' +
            'client_id=' + encodeURI(client_id) + '&' +
            'redirect_uri=' + encodeURI(redirect_uri) + '&' +
            'scope=' + encodeURI(scope) + '&' +
            'nonce=' + encodeURI(nonce) + '&' +
            'state=' + encodeURI(state);

        window.location.href = url;
    }

    authorizedCallback() {
        //this.resetAuthorizationData();

        let hash = window.location.hash.substr(1);

        let result = hash.split('&').reduce(function (result, item) {
            let parts = item.split('=');
            result[parts[0]] = parts[1];
            return result;
        }, {});

        console.log(result);

        let token = '';
        let id_token = '';
        let authResponseIsValid = false;

        if (!result.error) {

            if (result.state !== localStorage.getItem('authStateControl')) {
                console.log('AuthorizedCallback incorrect state');
            } else {

                token = result.access_token;
                id_token = result.id_token;

                let dataIdToken = this.getDataFromToken(id_token);

                // validate nonce
                if (dataIdToken.nonce !== localStorage.getItem('authNonce')) {
                    console.log('AuthorizedCallback incorrect nonce');
                } else {
                    localStorage.setItem('authNonce', '');
                    localStorage.setItem('authStateControl', '');

                    authResponseIsValid = true;
                    console.log('AuthorizedCallback state and nonce validated, returning access token');
                }
            }
        }

        if (authResponseIsValid) {
            this.setAuthorizationData(token, id_token);
            window.history.pushState({}, null, window.location.href.split('#')[0]);
        }
    }

    logoff() {
        let authorizationUrl = this.authorityUrl + '/connect/endsession';
        let id_token_hint = localStorage.getItem('authorizationDataIdToken');
        let post_logout_redirect_uri = this.location.origin + '/';

        let url =
            authorizationUrl + '?' +
            'id_token_hint=' + encodeURI(id_token_hint) + '&' +
            'post_logout_redirect_uri=' + encodeURI(post_logout_redirect_uri);

        this.resetAuthorizationData();

        // emit observable
        this.authenticationSource.next(false);
        window.location.href = url;
    }

    handleError(error) {
        console.log(error);
        if (error.status === 403) {
            this.history.push('/Forbidden');
        }
        else if (error.status === 401) {
            this.history.push('/Unauthorized');
        }
    }

    urlBase64Decode(str) {
        let output = str.replace('-', '+').replace('_', '/');
        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += '==';
                break;
            case 3:
                output += '=';
                break;
            default:
                throw 'Illegal base64url string!';
        }

        return window.atob(output);
    }

    getDataFromToken(token) {
        let data = {};

        if (typeof token !== 'undefined') {
            let encoded = token.split('.')[1];
            
            data = JSON.parse(this.urlBase64Decode(encoded));
        }

        return data;
    }

    async getUserData () {
        if (this.authorityUrl === '') {
            this.authorityUrl = localStorage.getItem('IdentityUrl');
        }

        const options = this.setHeaders();
        return await axios.get(`${this.authorityUrl}/connect/userinfo`, { headers: options})
    }

    setHeaders() {
        const token = this.getToken();
        if (token !== '') {
            this.headers.Authorization = `Bearer ${token}`;
        }
        return this.headers;
    }
}
import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import * as MicrosoftGraph from '@microsoft/microsoft-graph-types'
import { environment } from 'src/environments/environment';

// defining this interface we are saying that the response
// from the all users endpoint ('https://graph.microsoft.com/v1.0/users')
// has a field value of type T (see getUsers())
interface IODataResult<T> {
  value: T;
}

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styles: []
})
export class AppComponent implements OnInit {
  loggedIn = false;
  profile?: MicrosoftGraph.User;
  users?: MicrosoftGraph.User[];
  textToFilter = "" ;
  orders?: string[];

  constructor(private authService: MsalService,
    private client: HttpClient) {}
    
  ngOnInit(): void {
    this.checkAccount();
  }

  private checkAccount() {
    this.loggedIn = this.authService.instance.getAllAccounts().length > 0;
  }

  login() {
    this.authService.loginPopup()
      .subscribe(response => {
        this.authService.instance.setActiveAccount(response.account);
        this.checkAccount();
      });
  }

  logout() {
    this.authService.logout();
  }

  getProfile() {
    this.client.get('https://graph.microsoft.com/v1.0/me/')
      .subscribe(response => this.profile = response);
  }

  // Query MSGraph using **OData** standard
  getUsers() {
    let params = new HttpParams();
    //params = params.set("$top", "10"); // This will get only top 10 results
    if(this.textToFilter)
    {
      params = params.set(
        "$filter",
        `startsWith(displayName, '${this.textToFilter}')`,
      );
    }

    let url = `https://graph.microsoft.com/v1.0/users?${params.toString()}`;
    this.client
      // Two ways to getting the data from the field value of the response:
      .get<IODataResult<MicrosoftGraph.User[]>>(url) // Define an interface with the field value
      // .get<any>(url) // or say that the response is <any>
      .subscribe(response => this.users = response.value)
  }

  getOrders() {
    this.client.get<string[]>(`${environment.customApi}/orders`)
      .subscribe(response => this.orders = response);
  }
}

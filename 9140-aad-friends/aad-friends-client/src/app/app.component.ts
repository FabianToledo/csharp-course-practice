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

interface FriendId {
  id?: number;
  aadId: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  styles: []
})
export class AppComponent implements OnInit {
  loggedIn = false;
  profile?: MicrosoftGraph.User;
  aadSearchedUsers?: MicrosoftGraph.User[];
  allAadUsers?: MicrosoftGraph.User[];
  textToFilter = "" ;
  friendsId?: FriendId[];


  constructor(private authService: MsalService,
    private client: HttpClient) {}
    
  ngOnInit(): void {
    this.checkAccount();
    this.getAllAadUsers();
    this.getFriends();
  }

  private checkAccount() {
    this.loggedIn = this.authService.instance.getAllAccounts().length > 0;
    if (this.loggedIn)
      this.getProfile();
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

  getAllAadUsers(textToFilter: string = "") {
    this.getUsersObs(textToFilter).subscribe(response => this.allAadUsers = response.value);
  }

  getAadSearchedUsers(textToFilter: string = "") {
    this.getUsersObs(textToFilter).subscribe(response => this.aadSearchedUsers = response.value);
  }

  // Query MSGraph using **OData** standard
  getUsersObs(textToFilter: string = "") {
    let params = new HttpParams();
    //params = params.set("$top", "10"); // This will get only top 10 results
    if(textToFilter)
    {
      params = params.set(
        "$filter",
        `startsWith(displayName, '${textToFilter}')`,
      );
    }

    let url = `https://graph.microsoft.com/v1.0/users?${params.toString()}`;
    return this.client
      // Two ways to getting the data from the field value of the response:
      .get<IODataResult<MicrosoftGraph.User[]>>(url) // 1) We have defined an interface with the field value
      // .get<any>(url) // 2) another way to do it is to say that the response is <any>
      ;
  }

  mapFriends = () =>
    this.allAadUsers?.filter((u) => 
      this.friendsId?.map(f => f.aadId).includes(u.id ?? ""));
  

  getFriends() {
    this.client.get<FriendId[]>(`${environment.customApi}/friends`)
      .subscribe(response => this.friendsId = response);
  }

  addFriend(friendAadId?: string) {
    let friend:FriendId = { aadId : friendAadId ?? "" };

    this.client.put(`${environment.customApi}/friends`, friend)
      .subscribe(response => window.location.reload());
  }

  removeFriend(friendAadId?: string) {
    // let friend:FriendId = { aadId : friendAadId ?? "" };

    this.client.delete(`${environment.customApi}/friends/${friendAadId}`)
      .subscribe(response => window.location.reload());
  }
  
}

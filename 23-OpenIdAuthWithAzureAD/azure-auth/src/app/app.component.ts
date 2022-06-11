import { Component, OnInit } from '@angular/core';
import { MsalService } from '@azure/msal-angular';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styles: []
})
export class AppComponent implements OnInit {
  loggedIn = false;

  constructor(private authService: MsalService) {}

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
}

import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
   RegisterMode = false;
  /** values: any;*/
  constructor(private http: HttpClient) { }

  ngOnInit() {
    /**  this.GetValues();*/
  }
  ToggleRegister()
  {
    this.RegisterMode = true;
  }

 /**  GetValues()
  {
    this.http.get( 'http://localhost:5000/api/Values').subscribe(Response => {
      this.values = Response;
    }, error => { console.log(error); }
    );
  }*/
  CancelRegisterMode(registermode: boolean )
  {
    this.RegisterMode = registermode;
    console.log('reach here22');
  }

}

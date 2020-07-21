import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
model: any = {};
@Output() CancelRegister = new EventEmitter();
  constructor(private Authservice: AuthService) { }

  ngOnInit() {
  }

  Register()
  {
    this.Authservice.Register(this.model).subscribe(() => {
        console.log('registered');
    }, error => {console.log('error occured');}
    );
  }
  cancelled()
  {
    console.log('reach here');
    this.CancelRegister.emit(false);
  }

}

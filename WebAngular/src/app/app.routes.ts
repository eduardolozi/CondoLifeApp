import { Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';

export const routes: Routes = [
    {path: '', redirectTo: 'register', pathMatch: 'full'},
    {path: 'home', title: 'Home'},
    {path: 'register', title: 'Registrar', component: RegisterComponent}
];

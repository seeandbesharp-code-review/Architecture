// import { Component, signal } from '@angular/core';
// import { FormsModule } from '@angular/forms';
// import { RouterOutlet } from '@angular/router';

// @Component({
//   selector: 'app-root',
//   imports: [RouterOutlet,FormsModule],
//   templateUrl: './app.html',
//   styleUrl: './app.scss'
// })
// export class App {
//   protected readonly title = signal('AngularProject');
// }
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, RouterOutlet } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { HeaderComp } from './Components/header-comp/header-comp';
import { DropdownModule } from 'primeng/dropdown';
import { CommonModule } from '@angular/common'; // במקום BrowserModule
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    FormsModule,
    // RouterModule, // אם את משתמשת ב‑routing
    HeaderComp,
    DropdownModule,
    CommonModule
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App {
  protected readonly title = signal('AngularProject');
}

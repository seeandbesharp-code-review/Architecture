import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withFetch } from '@angular/common/http'; // ייבוא HttpClient
import { provideAnimations } from '@angular/platform-browser/animations';
import { DataViewModule } from 'primeng/dataview';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { Rating } from 'primeng/rating';


// ייבוא של PrimeNG
// import { providePrimeNG } from 'primeng/config';
import { PrimeNGConfig } from 'primeng/api';
// import Aura from '@primeuix/themes/aura';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(), // הוסיפי את זה כאן!
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), 
    provideClientHydration(withEventReplay()),
    
    // הוספת HttpClient עם תמיכה ב-Fetch (קריטי ל-SSR)
    provideHttpClient(withFetch()), 
    provideAnimations(),
    // הוספת PrimeNG למערך ה-providers
    // providePrimeNG({
    //     theme: {
    //         preset: Aura
    //     }
    // })
  ]
};
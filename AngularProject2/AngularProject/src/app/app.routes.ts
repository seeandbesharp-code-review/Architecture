import { Routes } from '@angular/router';
import { CartComp } from './Components/cart-comp/cart-comp';
// import { WorkerComp } from './Components/worker-comp/worker-comp';
// import { GetTest } from './Components/get-test/get-test';
// import { AddWorkTest } from './Components/add-work-test/add-work-test';
// import { AddBossTest } from './Components/add-boss-test/add-boss-test';
import { AuthComp } from './Components/auth-comp/auth-comp';
import { HomeComp } from './Components/home-comp/home-comp';
import { AllGiftsComp } from './Components/all-gifts-comp/all-gifts-comp';
import { DonorsComp } from './Components/donors-comp/donors-comp';
import { DonorGiftsComp } from './Components/donor-gifts-comp/donor-gifts-comp';
import { GiftDetailComp } from './Components/gift-detail-comp/gift-detail-comp';
import { ManagerGifts } from './Components/manager-gifts/manager-gifts';
import { PaymentProcessComponent } from './Components/payment-process-component/payment-process-component';

export const routes: Routes = [

    { path: 'user', component: AuthComp },
    { path: 'gift/:id', component: GiftDetailComp },
    { path: '', component: HomeComp },
    { path: 'all-gifts', component: AllGiftsComp },
    { path: 'all-gifts-manager', component: ManagerGifts },
    // { path: 'checkout', component: Ch },
    { path: 'payment', component: PaymentProcessComponent },

    { path: 'cart', component: CartComp },
    { path: 'donors', component: DonorsComp },
    { path: 'donor-gifts/:id', component: DonorGiftsComp }
];

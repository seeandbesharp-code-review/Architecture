import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { Menubar } from 'primeng/menubar';
import { MenubarModule } from 'primeng/menubar'; // במקום 'primeng/api' או ייבוא פשוט
import { AuthorService } from '../../Services/AutorService';

@Component({
  selector: 'app-header-comp',
  imports: [MenubarModule, RouterLink],
  templateUrl: './header-comp.html',
  styleUrl: './header-comp.scss',
})
export class HeaderComp implements OnInit {

  items: MenuItem[] = [];

  constructor(private authorService: AuthorService) { }

  ngOnInit() {

    this.buildMenu(this.authorService.isManager());

    this.authorService.role$.subscribe(role => {
      this.buildMenu(role?.toLowerCase() === 'manager');
    });

    console.log('Is manager?', this.authorService.isManager());
  }

  buildMenu(isManager: boolean): void {

    this.items = [
      { label: 'Home', icon: 'pi pi-home', routerLink: '/' },
      { label: 'AllGifts', icon: 'pi pi-gift', routerLink: '/all-gifts' },
      { label: 'My Cart', icon: 'pi pi-home', routerLink: '/cart' },
      { label: 'User', icon: 'pi pi-user', routerLink: '/user' }
    ];

    if (isManager) {
      this.items.push(
        { label: 'All Donors', icon: 'pi pi-users', routerLink: '/donors' },
        { label: 'All gifts for manager', icon: 'pi pi-users', routerLink: '/all-gifts-manager' }
      );
    }
  }
}

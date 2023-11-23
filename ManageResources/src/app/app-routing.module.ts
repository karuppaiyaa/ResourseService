import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResourcesListComponent } from './resources-list/resources-list.component';

const routes: Routes = [
  {
    path: 'resources',
    component: ResourcesListComponent
  },  
  {
    path:'', redirectTo:'resources',pathMatch:'full'
  },
  {
    path:'**', component:ResourcesListComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

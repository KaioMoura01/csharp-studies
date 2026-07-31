import { apiService } from '../services/api_service'
import {ref} from "vue";
import type {components} from "@/types/api-schema";

export function DashboardService(){

  type MyAccount = components['schemas']['AccountDetailsResponse'];

  const loading = ref(false);
  const error = ref('');
  const myAccount = ref<MyAccount>();

  async function GetData(){
    try{
      loading.value = true;
      const {data} = await apiService.get<MyAccount>("accounts/me");
      myAccount.value = data;
    } catch{
      error.value = "Erro ao buscar seus dados, por favor tente mais tarde";
    } finally {
      loading.value = false;
    }
  }

  return {myAccount, error, loading, GetData}
}

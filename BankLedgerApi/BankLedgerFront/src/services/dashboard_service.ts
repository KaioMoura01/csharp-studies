import { apiService } from '../services/api_service'
import {ref} from "vue";
import type {components} from "@/types/api-schema";
import { useBalanceStore } from '@/stores/balance_store'

export function DashboardService(){

  type MyAccount = components['schemas']['AccountDetailsResponse'];

  const balanceStore = useBalanceStore();
  const loading = ref(false);
  const error = ref('');
  const myAccount = ref<MyAccount>();

  async function GetData(){
    try{
      loading.value = true;
      const {data} = await apiService.get<MyAccount>("accounts/me");
      myAccount.value = data;
      balanceStore.setAvailable(data.currentBalance);
    } catch{
      error.value = "Erro ao buscar seus dados, por favor tente mais tarde";
    } finally {
      loading.value = false;
    }
  }

  return {myAccount, error, loading, GetData}
}

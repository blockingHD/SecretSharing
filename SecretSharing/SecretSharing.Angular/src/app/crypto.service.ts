import { Injectable } from '@angular/core';
import { Buffer } from 'buffer';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  getMessageEncoding(message: string) {
    let enc = new TextEncoder();
    return enc.encode(message);
  }

  /*
  Get some key material to use as input to the deriveKey method.
  The key material is a password supplied by the user.
  */
  getKeyMaterial(password: string) {
    if (!password) {
      throw new Error("No Password was given.");
    }

    let enc = new TextEncoder();
    return window.crypto.subtle.importKey(
      "raw",
      enc.encode(password),
      {name: "PBKDF2"},
      false,
      ["deriveBits", "deriveKey"]
    );
  }

  /*
  Given some key material and some random salt
  derive an AES-GCM key using PBKDF2.
  */
  getKey(keyMaterial: CryptoKey, salt: Uint8Array) {
    return window.crypto.subtle.deriveKey(
      {
        "name": "PBKDF2",
        salt: salt,
        "iterations": 210000,
        "hash": "SHA-512"
      },
      keyMaterial,
      { "name": "AES-GCM", "length": 256},
      true,
      [ "encrypt", "decrypt" ]
    );
  }

  /*
  Generate RSA key pair
   */
  async generateRSAKeyPair(): Promise<CryptoKeyPair>  {
    return window.crypto.subtle.generateKey(
      {
        name: "RSA-OAEP",
        modulusLength: 2048,
        publicExponent: new Uint8Array([1, 0, 1]),
        hash: "SHA-256"
      },
      true,
      ["encrypt", "decrypt"]
    );
  }

  /*
  Derive a key from a password supplied by the user, and use the key
  to encrypt the message.
  Update the "ciphertextValue" box with a representation of part of
  the ciphertext.
  */
  async encrypt(password: string): Promise<KeyMaterial> {
    const rsaKeyPair = await this.generateRSAKeyPair();

    let derivationKey = await this.getKeyMaterial(password);
    const salt = window.crypto.getRandomValues(new Uint8Array(16));
    let key = await this.getKey(derivationKey, salt);
    const iv = window.crypto.getRandomValues(new Uint8Array(12));

    let ciphertext = await window.crypto.subtle.encrypt(
      {
        name: "AES-GCM",
        iv: iv
      },
      key,
      await crypto.subtle.exportKey("pkcs8", rsaKeyPair.privateKey)
    );

    return new KeyMaterial(
      new Uint8Array(await crypto.subtle.exportKey("spki", rsaKeyPair.publicKey)),
      new Uint8Array(ciphertext),
      salt,
      iv
    );
  }

  /*
  Derive a key from a password supplied by the user, and use the key
  to decrypt the ciphertext.
  If the ciphertext was decrypted successfully,
  update the "decryptedValue" box with the decrypted value.
  If there was an error decrypting,
  update the "decryptedValue" box with an error message.
  */
  async decrypt(keyMaterial: KeyMaterial, password: string) {
    let derivationKey = await this.getKeyMaterial(password);
    let key = await this.getKey(derivationKey, keyMaterial.salt!);

    try {
      let decrypted = await window.crypto.subtle.decrypt(
        {
          name: "AES-GCM",
          iv: keyMaterial.iv!
        },
        key,
        keyMaterial.encryptedPrivateKey
      );

      return decrypted;
    } catch (e) {
      throw new Error("Failed to decrypt.");
    }
  }
}

class KeyMaterial {
  public publicKey: Uint8Array;
  public encryptedPrivateKey: Uint8Array;
  public salt: Uint8Array;
  public iv: Uint8Array;

  constructor(publicKey: Uint8Array, encryptedPrivateKey: Uint8Array, salt: Uint8Array, iv: Uint8Array) {
    this.publicKey = publicKey;
    this.encryptedPrivateKey = encryptedPrivateKey;
    this.salt = salt;
    this.iv = iv;
  }

  get json(): string {
    return JSON.stringify({
      publicKey: Buffer.from(this.publicKey).toString('base64'),
      encryptedPrivateKey: Buffer.from(this.encryptedPrivateKey).toString('base64'),
      salt: Buffer.from(this.salt).toString('base64'),
      iv: Buffer.from(this.iv).toString('base64')
    });
  }
}
